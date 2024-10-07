using Chat.Api.Context;
using Chat.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Chat.Api.Repositories;

public class ChatRepository(ChatDbContext context) : IChatRepository
{
    private readonly ChatDbContext _context = context;

    public async Task<List<Entities.Chat>> GetAllChats()
    {
        var chats = await _context.Chats.AsNoTracking().ToListAsync();
        return chats;
    }

    public async Task<List<Entities.Chat>> GetAllUserChats(Guid userId)
    {
        var userChats = 
            await _context.UserChats.
                Where(c => c.UserId == userId)
                .ToListAsync();

        List<Entities.Chat> sortedChats = new();
        var check = userChats == null || userChats.Count == 0;
        if (check)
            return sortedChats; 

        foreach (var chat in userChats)
        {
            var sortedChat = await
                _context.Chats
                    .Include(ch => ch.Messages)!
                    .ThenInclude(m => m.Content)
                    .SingleAsync(ch => ch.Id == chat.ChatId);
            sortedChats.Add(sortedChat);
        }
        return sortedChats;
    }

    public async Task<Entities.Chat> GetUserChatById(Guid userId, Guid chatId)
    {
        var userChat = await _context.UserChats
            .SingleOrDefaultAsync
            (uc => uc.UserId == userId &&
                                        uc.ChatId == chatId);

        if (userChat is null)
            throw new ChatNotFoundException();

        var chat = await
            _context.Chats.
                SingleAsync(c => c.Id == userChat.ChatId);
        return chat;
    }

    public async Task<Tuple<bool,Entities.Chat?>> CheckChatExist(Guid fromUserId, Guid toUserId)
    {
        var userChat = await _context.UserChats
            .FirstOrDefaultAsync(uc => uc.UserId == fromUserId 
                                        && uc.ToUserId == toUserId);


        if (userChat is not null)
        {
            var chat = await GetUserChatById(userChat.UserId, userChat.ChatId);
            return new(true, chat);
        }

        return new(false, null);

    }

    public async Task AddChat(Entities.Chat chat)
    {
       // await _context.AddAsync(chat);

        await _context.Chats.AddAsync(chat);

       await _context.SaveChangesAsync();
    }

    public async Task UpdateChat(Entities.Chat chat)
    {
        _context.Chats.Update(chat);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteChat(Entities.Chat chat)
    {
        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync();
    }
}