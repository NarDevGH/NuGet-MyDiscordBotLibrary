// <copyright file="DiscordTextChannel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyDiscordBotLibrary;

using Discord;
using Discord.WebSocket;
using Discord.Interactions;

public class DiscordTextChannel
{
    public static SocketCategoryChannel? ChannelCategory(SocketInteractionContext context, ITextChannel channel)
    {
        if (channel is null) return null;

        return context.Guild.GetCategoryChannel((ulong)channel.CategoryId);
    }
}
