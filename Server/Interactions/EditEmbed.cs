using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;

namespace Server.Interactions;

public class EditEmbed : InteractionModuleBase<SocketInteractionContext<SocketMessageCommand>>
{
    private readonly IMemoryCache cache;

    public EditEmbed(IMemoryCache cache)
    {
        this.cache = cache;
    }

    [MessageCommand("Edit Embed")]
    public async Task EditEmbedCommand(IMessage msg)
    {
        if (msg is not IUserMessage usrMsg) return;

        var embed = usrMsg.Embeds?.FirstOrDefault();
        if (embed == null || usrMsg.Author.Id != Context.Client.CurrentUser.Id)
        {
            await RespondAsync($"That message cannot be edited", ephemeral: true).ConfigureAwait(false);
            return;
        }

        int fieldCount = 0;

        var modal = new ModalBuilder().WithTitle($"Edit embed - {embed.Title}").WithCustomId($"edit-embed-main:{usrMsg.Id}");
        modal.AddTextInput("Embed Title", GenericInputModal.GetGenericIdWord(fieldCount), TextInputStyle.Short, value: embed.Title, required: false, maxLength: EmbedBuilder.MaxTitleLength);
        modal.AddTextInput("Description", GenericInputModal.GetGenericIdWord(++fieldCount), TextInputStyle.Paragraph, value: embed.Description, required: false, maxLength: 4000);
        modal.AddTextInput("Url", GenericInputModal.GetGenericIdWord(++fieldCount), TextInputStyle.Short, value: embed.Thumbnail?.Url, required: false);
        modal.AddTextInput("Author Field", GenericInputModal.GetGenericIdWord(++fieldCount), TextInputStyle.Short, value: embed.Author?.Name, required: false);
        modal.AddTextInput("Footer", GenericInputModal.GetGenericIdWord(++fieldCount), TextInputStyle.Paragraph, value: embed.Footer?.Text, required: false, placeholder: "markdown isn't supported in the footer", maxLength: EmbedFooterBuilder.MaxFooterTextLength);

        await RespondWithModalAsync(modal.Build());
    }

    [MessageCommand("Edit Embed - Fields")]
    public async Task EditEmbedFieldsCommand(IMessage msg)
    {
        if (msg is not IUserMessage usrMsg) return;

        var embed = usrMsg.Embeds?.FirstOrDefault();
        if (embed == null || usrMsg.Author.Id != Context.Client.CurrentUser.Id)
        {
            await RespondAsync($"That message cannot be edited", ephemeral: true).ConfigureAwait(false);
            return;
        }

        var guid = Guid.NewGuid().ToString("N");

        int fieldCount = 0;

        var modal = new ModalBuilder().WithTitle($"Edit embed - {embed.Title}").WithCustomId($"edit-embed-fields:{usrMsg.Id}:{guid}");

        var fieldList = new List<ModalFieldData>();
        foreach (var field in embed.Fields)
        {
            if (fieldCount >= ModalComponentBuilder.MaxActionRowCount) continue;

            var fieldData = new ModalFieldData(field.Name, fieldCount, field.Value, embed.Fields.IndexOf(field), "warning: emptying this field will cause the field to be deleted.");
            fieldCount++;

            fieldList.Add(fieldData);
            modal.AddTextInput(fieldData.Build());
        }

        //TODO: This is to let the player add a field, but it's not working because of some index misalignment.
        //if (fieldList.Count <= ModalComponentBuilder.MaxActionRowCount - 2)
        //{
        //    var titleField = new ModalFieldData("New Field Title", fieldCount, null, null);
        //    fieldCount++;
        //    modal.AddTextInput(titleField.Build());
        //    fieldList.Add(titleField);

        //    var data = new ModalFieldData("New Field Value", fieldCount, null, null);
        //    modal.AddTextInput(data.Build());
        //    fieldList.Add(data);
        //}

        cache.Set(guid, fieldList, DateTimeOffset.Now.AddMinutes(30));

        await RespondWithModalAsync(modal.Build());
    }
}

public class EditEmbedComponents : InteractionModuleBase
{
    private readonly IMemoryCache cache;

    public EditEmbedComponents(IMemoryCache cache)
    {
        this.cache = cache;
    }

    [ModalInteraction("edit-embed-fields:*:*")]
    public async Task ModalResponse(ulong messageId, string guid, GenericInputModal<string, string, string> modal)
    {
        await EditModalHandler(messageId, guid, modal.First, modal.Second, modal.Third);
    }

    [ModalInteraction("edit-embed-fields:*:*")]
    public async Task ModalResponse(ulong messageId, string guid, GenericInputModal<string, string, string, string> modal)
    {
        await EditModalHandler(messageId, guid, modal.First, modal.Second, modal.Third, modal.Fourth);
    }

    [ModalInteraction("edit-embed-fields:*:*")]
    public async Task ModalResponse(ulong messageId, string guid, GenericInputModal<string, string, string, string, string> modal)
    {
        await EditModalHandler(messageId, guid, modal.First, modal.Second, modal.Third, modal.Fourth, modal.Fifth);
    }

    [ModalInteraction("edit-embed-main:*")]
    public async Task ModalResponseMain(ulong messageId, GenericInputModal<string, string, string, string, string> modal)
    {
        if (await Context.Channel.GetMessageAsync(messageId) is not IUserMessage message) return;

        var embed = message.Embeds.FirstOrDefault().ToEmbedBuilder();

        embed.WithTitle(modal.First);
        embed.WithDescription(!String.IsNullOrWhiteSpace(modal.Second) ? modal.Second : null);
        embed.WithUrl(!String.IsNullOrWhiteSpace(modal.Third) ? modal.Third : null);
        embed.WithAuthor(!String.IsNullOrWhiteSpace(modal.Fourth) ? modal.Fourth : null);
        embed.WithFooter(!String.IsNullOrWhiteSpace(modal.Fifth) ? modal.Fifth : null);

        await message.ModifyAsync(msg => msg.Embed = embed.Build());
        await RespondAsync();
    }

    public async Task EditModalHandler(ulong messageId, string guid, params string[] values)
    {
        var downloadedMsg = await Context.Channel.GetMessageAsync(messageId);
        if (downloadedMsg is not IUserMessage message)
        {
            await RespondAsync("I can't edit that message, try again with a message that was created by the bot", ephemeral: true);
            return;
        }

        var embed = message.Embeds.FirstOrDefault().ToEmbedBuilder();

        if (cache.TryGetValue(guid, out List<ModalFieldData> cachedModalData))
        {
            for (var i = cachedModalData.Count - 1; i >= 0; i--)
            {
                var fieldData = cachedModalData[i];
                if (fieldData.embedIndex >= 0 && embed.Fields.Count - 1 >= fieldData.embedIndex)
                {
                    if (string.IsNullOrWhiteSpace(values[fieldData.ModalFieldNumber]))
                    {
                        embed.Fields.RemoveAt(fieldData.embedIndex);
                    }
                    else
                    {
                        embed.Fields[fieldData.embedIndex].Value = values[fieldData.ModalFieldNumber]; 
                    }
                }
                //else //TODO: this is the other part of the Add field feature
                //{
                //    if (fieldData.embedIndex < 0
                //        && values.Length >= fieldData.ModalFieldNumber
                //        && !string.IsNullOrWhiteSpace(values[fieldData.ModalFieldNumber])
                //        && !string.IsNullOrWhiteSpace(values[fieldData.ModalFieldNumber + 1]))
                //    {
                //        embed.AddField(values[fieldData.ModalFieldNumber], values[fieldData.ModalFieldNumber + 1]);
                //        break;
                //    }
                //}
            }
        }

        await message.ModifyAsync(msg => msg.Embed = embed.Build());

        await RespondAsync();
        cache.Remove(guid);
    }
}

public class ModalFieldData
{
    public ModalFieldData(string fieldName, int modalFieldNumber, string? value, int? embedFieldNumber, string? placeHolder = null)
    {
        FieldName = fieldName;
        ModalFieldNumber = modalFieldNumber;
        Value = value;
        embedIndex = embedFieldNumber ?? -1;
        PlaceHolder = placeHolder;
    }

    public int ModalFieldNumber { get; set; }

    public string FieldName { get; set; }

    public string? Value { get; set; }
    public int embedIndex { get; set; }
    public string? PlaceHolder { get; }

    public TextInputBuilder Build()
    {
        return new TextInputBuilder()
            .WithCustomId(GenericInputModal.GetGenericIdWord(ModalFieldNumber))
            .WithLabel(FieldName)
            .WithValue(Value)
            .WithPlaceholder(PlaceHolder)
            .WithRequired(false)
            ;
    }
}
