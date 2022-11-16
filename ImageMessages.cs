// <copyright file="ImageMessages.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyDiscordBotLibrary;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

public static class ImageMessage
{
    /// <summary>
    /// Returns true if the file extension belong to a image file, otherwise returns false.
    /// </summary>
    /// <param name="filename"> The file name.</param>
    /// <returns>Returns true or false depending if the file is an image.</returns>
    public static bool IsImageFile(string filename)
    {
        filename = filename.ToLower();
        var res = filename.EndsWith("png") || filename.EndsWith("jpg") || filename.EndsWith("jpeg");
        return res;
    }

    /// <summary>
    /// Returns true if the given message is an ImageMessagem.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The executuion result of the module.</returns>
    public static bool IsImageMessage(IMessage message)
    {
        if (message.Attachments == null || message.Attachments.Count == 0)
        {
            return IsImageFile(message.Content);
        }

        var firstAttachment = message.Attachments.First();
        return IsImageFile(firstAttachment.Filename);
    }

    /// <summary>
    /// Returns a list containing all the images url from the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The executuion result of the module.</returns>
    public static List<string> ListOf_ImagesUrlFromMessage(IMessage message)
    {
        return message.Attachments.Where(x => IsImageFile(x.Filename)).Select(x => x.Url).ToList();
    }

    /// <summary>
    /// Returns a list containing all the images url from the messages.
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <returns>The executuion result of the module.</returns>
    public static List<string> ListOf_ImagesUrlFromMessages(IEnumerable<IMessage> messages)
    {
        List<string> result = new List<string>();

        foreach (var message in messages)
        {
            result.AddRange(ListOf_ImagesUrlFromMessage(message));
        }

        return result;
    }

    /// <summary>
    /// Returns a list containing the requested ammount of images url from the messages.
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="ammount">The ammount of images urls requested.</param>
    /// <returns>The executuion result of the module.</returns>
    public static List<string> ListOf_ImagesUrlFromMessages(IEnumerable<IMessage> messages, int ammount)
    {
        List<string> result = new List<string>();

        int counter = 0;
        foreach (var message in messages)
        {
            var images = ListOf_ImagesUrlFromMessage(message);

            counter += images.Count;

            if (counter > ammount)
            {
                images = images.Take(images.Count - (counter - ammount)).ToList();
            }

            result.AddRange(images);

            if (counter >= ammount) break;
        }

        return result;
    }

    /// <summary>
    /// Returns a list containing all the images url from the images attachments on all the channel messages.
    /// </summary>
    /// <param name="context">The SocketCommandContext.</param>
    /// <param name="channel">The SocketMessageChannel.</param>
    /// <param name="cacheSize">The Cache Size.</param>
    /// <returns>The executuion result of the module.</returns>
    public static async Task<List<string>> ListOf_ChannelImagesUrl(SocketCommandContext context, ISocketMessageChannel channel, int cacheSize)
    {
        List<string> result = new List<string>();

        IEnumerable<IMessage> channelMessages;

        do
        {
            channelMessages = await channel.GetMessagesAsync(
            fromMessage: context.Message,
            dir: Direction.Before,
            limit: cacheSize,
            mode: CacheMode.AllowDownload)
            .FlattenAsync();

            result.AddRange(ListOf_ImagesUrlFromMessages(channelMessages));
        }
        while (channelMessages.Count() == cacheSize);

        return result;
    }

    /// <summary>
    /// Returns a list containing the requested ammount of images url from the images attachments on all the channel messages.
    /// </summary>
    /// <param name="channel">The SocketMessageChannel.</param>
    /// <param name="ammount">The ammount of images urls.</param>
    /// <param name="cacheSize">The Cache Size.</param>
    /// <param name="context">The SocketCommandContext.</param>
    /// <returns>The executuion result of the module.</returns>
    public static async Task<List<string>> ListOf_ChannelImagesUrl(ISocketMessageChannel channel, int ammount, int cacheSize, SocketCommandContext context)
    {
        List<string> result = new List<string>();

        IEnumerable<IMessage> channelMessages;
        int counter = 0;
        do
        {
            channelMessages = await channel.GetMessagesAsync(
            fromMessage: context.Message,
            dir: Direction.Before,
            limit: cacheSize,
            mode: CacheMode.AllowDownload)
            .FlattenAsync();

            var images = ListOf_ImagesUrlFromMessages(channelMessages);

            counter += images.Count;
            if (counter > ammount)
            {
                images = images.Take(images.Count - (counter - ammount)).ToList();
            }

            result.AddRange(images);
        }
        while (counter < ammount && channelMessages.Count() == cacheSize);

        return result;
    }

    public static async Task<List<ulong>> ListOf_ChannelImageMessagesID(ISocketMessageChannel channel, IMessage fromMessage, int cacheSize)
    {
        List<ulong> messagesID = new();

        IMessage lastMessageReached = fromMessage;
        IEnumerable<IMessage> channelMessages;
        do
        {
            channelMessages = await channel.GetMessagesAsync(
            fromMessage: lastMessageReached,
            dir: Direction.Before,
            limit: cacheSize,
            mode: CacheMode.AllowDownload)
            .FlattenAsync();

            if (channelMessages == null) break;

            messagesID.AddRange(channelMessages.Where(message => IsImageMessage(message))
                                               .Select(x => x.Id).ToList());

            lastMessageReached = channelMessages.Last();
        }
        while (channelMessages.Count() == cacheSize);

        return messagesID;
    }
}

