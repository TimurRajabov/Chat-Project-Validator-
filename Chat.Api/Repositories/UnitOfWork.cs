using Chat.Api.Context;

namespace Chat.Api.Repositories;

public class UnitOfWork(ChatDbContext context) : IUnitOfWork
{
    private IUserRepository? _userRepository { get;}

    public IUserRepository UserRepository => 
        _userRepository ?? new UserRepository(context);

    private IChatRepository? _chatRepository { get; }
    public IChatRepository ChatRepository 
    => _chatRepository ?? new ChatRepository(context);

    private IUserChatRepository? _userChatRepository { get; }
    public IUserChatRepository UserChatRepository =>
    _userChatRepository ?? new UserChatRepository(context);

    public IMessageRepository MessageRepository =>
        _messageRepository ?? new MessageRepository(context);

    private IMessageRepository? _messageRepository { get; }
}