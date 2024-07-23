using MediatR;

using Microsoft.EntityFrameworkCore;

using SisyphusServer.Database;

namespace SisyphusServer.Commands {
    public class ResetDatabaseCommand : IRequest<Unit> { }

    public class ResetDatabaseCommandHandler : IRequestHandler<ResetDatabaseCommand, Unit> {
        private readonly IDatabaseContext DatabaseContext;

        public ResetDatabaseCommandHandler(IDatabaseContext databaseContext) {
            DatabaseContext = databaseContext;
        }

        public async Task<Unit> Handle(ResetDatabaseCommand command, CancellationToken token) {
            await DatabaseContext.UserInfos.ExecuteDeleteAsync(token);
            return Unit.Value;
        }
    }
}
