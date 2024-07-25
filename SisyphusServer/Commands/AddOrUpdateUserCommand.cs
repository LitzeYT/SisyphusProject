using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SisyphusServer.Database;
using SisyphusServer.Database.Entities;

namespace SisyphusServer.Commands {
    public class AddOrUpdateUserCommand : IRequest<UserInfo> {
        public string UserId { get; set; }
        public long? Points { get; set; }
    }

    public class AddOrUpdateUserCommandValidator : AbstractValidator<AddOrUpdateUserCommand> {
        public AddOrUpdateUserCommandValidator() {
            RuleFor(command => command.UserId)
                .Must(userId => !string.IsNullOrWhiteSpace(userId)).WithMessage("No userid provided")
                .MinimumLength(3).WithMessage("Userid to small")
                .MaximumLength(64).WithMessage("Userid to long");

            RuleFor(command => command.Points)
                .Must(point => point.HasValue).WithMessage("No points provided")
                .GreaterThanOrEqualTo(0).WithMessage("Points can not be less 0");
        }
    }

    public class AddOrUpdateUserCommandHandler : IRequestHandler<AddOrUpdateUserCommand, UserInfo> {
        private readonly IDatabaseContext DatabaseContext;

        public AddOrUpdateUserCommandHandler(IDatabaseContext databaseContext) {
            DatabaseContext = databaseContext;
        }

        public async Task<UserInfo> Handle(AddOrUpdateUserCommand command, CancellationToken token) {
            var user = await DatabaseContext.UserInfos.FirstOrDefaultAsync(user => user.UserId == command.UserId, token);
            if (user == null) {
                user = new UserInfo { UserId = command.UserId, Points = command.Points!.Value };
                await DatabaseContext.UserInfos.AddAsync(user, token);
            }
            user.Points = command.Points!.Value;
            await DatabaseContext.SaveChangesAsync(token);
            return user;
        }
    }
}
