using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            Company old = _db.Companies.FirstOrDefault(c => c.Id == company.Id);
            if(old!=null)
            {
                old.Name = company.Name;
                old.StreetAddress = company.StreetAddress;
                old.City = company.City;
                old.PostalCode = company.PostalCode;
                old.State = company.State;
                old.PhoneNumber = company.PhoneNumber;
            }
        }
    }
}
