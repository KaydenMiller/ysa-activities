using LaytonYSAClerk.Cli.Entities;
using Microsoft.EntityFrameworkCore;

namespace LaytonYSAClerk.Cli.Services;

public class MembersRepository
{
    private readonly ChurchDbContext _dbContext;

    public MembersRepository(ChurchDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddMember(Member member)
    {
        var existingMember = _dbContext.Members.SingleOrDefault(m => m.MemberId == member.MemberId);
        if (existingMember is null)
        {
            // user does not yet exist add them
            _dbContext.Members.Add(member);
            await _dbContext.SaveChangesAsync(); 
        }
    }

    public Task UpdateMember(Member member)
    {
        _dbContext.Members.Update(member);
        return _dbContext.SaveChangesAsync();
    }

    public async Task<Member?> GetMember(long memberId)
    {
        var member = await _dbContext.Members.FindAsync(memberId);
        return member;
    }

    public Task<List<Member>> GetMembers()
    {
        return _dbContext.Members.ToListAsync();
    }

    public Task<List<Member>> GetMembersToEmail()
    {
        var members = _dbContext.Members
           .Where(m => m.NewMemberEmailSentDate == null)
           .Include(m => m.Unit);

        return members.ToListAsync();
    }
}