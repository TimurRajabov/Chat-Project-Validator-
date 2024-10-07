namespace Chat.Api.Repositories;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IChatRepository ChatRepository { get; }
    IUserChatRepository UserChatRepository { get; }
    IMessageRepository MessageRepository { get; }
}