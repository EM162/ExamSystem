using ITI.ExamSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace ITI.ExamSystem.Repository
{
    public class SpAdmin_Repo
    {
        OnlineExaminationDBContext _context;

        public SpAdmin_Repo(OnlineExaminationDBContext _context)
        {
            this._context = _context;
        }

        public List<Branch> GetAllBranches()
        {
           var branches= _context.Branches.ToList();
            return branches;
        }

        public Branch GetBranch_ByID(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var branch= _context.Branches.FirstOrDefault(b=>b.BranchID==id);
            return branch;
        }

        public void AddBranch(Branch branch)
        {
            if(branch == null) throw new ArgumentNullException(nameof(branch));

            _context.Branches.Add(branch);
            _context.SaveChanges();
        }

        public void Update(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var branch = GetBranch_ByID(id);
            if (branch == null) throw new InvalidOperationException($"Branch with ID {id} not found.");

            _context.Branches.Update(branch);
            _context.SaveChanges(true);
        }

        public void Delete(int? id)
        {
            if(id==null) throw new ArgumentNullException(nameof(id));

            var branch= GetBranch_ByID(id);
            if (branch == null) throw new InvalidOperationException($"Branch with ID {id} not found.");

            var relatedRecords = _context.IntakeBranchTrackUsers.Where(r => r.BranchID == id).ToList();
            _context.IntakeBranchTrackUsers.RemoveRange(relatedRecords);

            _context.Branches.Remove(branch);
            _context.SaveChanges(true);
        }

        public void DeleteUser(int id)
        {
            var result = _context.Users
          //.Include(u => u.IdentityUser)
           .FirstOrDefault(u => u.UserID == id);

            if (result == null) throw new InvalidOperationException();

            _context.Users.Remove(result);
            _context.SaveChanges(true);
        }


    }
}
