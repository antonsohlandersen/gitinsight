namespace GitInsight.Entities;
using GitInsight.Entities.DTOS;
using Microsoft.EntityFrameworkCore;
using GitInsight.Core;
using System.Linq;
    public class DBRepoRepository
    {
        private RepositoryContext _context;

        public DBRepoRepository(RepositoryContext context)
        {
            _context = context;
        }

        public Response Create(DBRepositoryDTO dto){
            var entity = _context.RepoData.Find(dto.Id);
            if (entity is null)
            {
                entity = new DBRepository{Id = dto.Id, state = dto.state, commits = dto.commits};
                _context.RepoData.Add(entity);
                _context.SaveChanges();
                return Response.Created;
            }
                return Response.Conflict;
        }

        public Response Update(DBUpdateRepositoryDTO dto) {
            var entity = _context.RepoData.Find(dto.Id);
            if (entity is null){
                return Response.NotFound;
            }
            else{ 
                var lastCommit = ReadLastCommit(new DBRepositoryDTO{Id = dto.Id});
                entity.Id = dto.Id;
                entity.state = dto.state;
                if (dto.commits is not null){
                    foreach (DBCommit commit in dto.commits){
                        if (commit.date > lastCommit.date){
                            entity.commits.Add(commit);
                        }
                    }
                }
                _context.SaveChanges();
                return Response.Updated;
            }
        }

        public DBLastCommitDTO? ReadLastCommit(DBRepositoryDTO dto){
            var repo = _context.RepoData.Where(x => x.Id == dto.Id).Include(x => x.commits).FirstOrDefault();

            if (repo is null)
            {
                return null;
            }
            else{
                return new DBLastCommitDTO (repo.commits.MaxBy(x => x.date).date);
            };
        }

        public DBReadRepositoryDTO? Read(string repoID){
            var repo = _context.RepoData.Find(repoID);
            return repo is null ? null : new DBReadRepositoryDTO{state = repo.state, commits = repo.commits};
        }

        public IEnumerable<DBReadCommitDTO> ReadAllCommits(string repoID){
            var repo = _context.RepoData.Where(x => x.Id == repoID).Include(x => x.commits).FirstOrDefault();

            if (repo is null)
            {
                return null;
            }
            else{
                return from c in repo.commits
                    select new DBReadCommitDTO{author = c.author, date = c.date};
            };
        }
    }