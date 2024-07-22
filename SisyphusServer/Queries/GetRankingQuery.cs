using MediatR;

using Microsoft.EntityFrameworkCore;

using SisyphusServer.Database;
using SisyphusServer.Database.Entities;

namespace SisyphusServer.Queries {
    public class GetRankingQuery : IRequest<List<UserInfo>> { }

    public class GetRankingQueryHandler : IRequestHandler<GetRankingQuery, List<UserInfo>> {
        private readonly IDatabaseContext DatabaseContext;

        public GetRankingQueryHandler(IDatabaseContext databaseContext) {
            DatabaseContext = databaseContext;
        }

        public async Task<List<UserInfo>> Handle(GetRankingQuery query, CancellationToken token) {
            return await DatabaseContext.UserInfos
                .OrderByDescending(user => user.Points)
                .ToListAsync(token);
        }
    }
}
