using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SisyphusServer.Database;
using SisyphusServer.Database.Entities;

namespace SisyphusServer.Queries {
    public class GetUserByIdQuery : IRequest<UserInfo> {
        public string UserId { get; set; }
    }

    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery> {
        public GetUserByIdQueryValidator(IDatabaseContext databaseContext) {
            RuleFor(query => query.UserId)
                .Must(userId => !string.IsNullOrWhiteSpace(userId)).WithMessage("No userid provided")
                .MinimumLength(3).WithMessage("Userid to small")
                .MaximumLength(64).WithMessage("Userid to long")
                .Must(userId => {
                    return databaseContext.UserInfos.Any(user => user.UserId == userId);
                }).WithMessage("User not found");
        }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserInfo> {
        private readonly IDatabaseContext DatabaseContext;

        public GetUserByIdQueryHandler(IDatabaseContext databaseContext) {
            DatabaseContext = databaseContext;
        }

        public async Task<UserInfo> Handle(GetUserByIdQuery query, CancellationToken token) {
            return await DatabaseContext.UserInfos.FirstAsync(user => user.UserId == query.UserId, token);
        }
    }
}
