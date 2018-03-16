using System.Threading.Tasks;
using tomware.Microwf;
using WebApi.Domain;
using WebApi.Models;

namespace WebApi.Services
{
  public interface IHolidayService
  {
    HolidayViewModel GetNew();
    Task<HolidayViewModel> GetAsync(int id);

    // Task<HolidayViewModel> ApplyAsync();
  }

  public class HolidayService : IHolidayService
  {
    private readonly DomainContext _context;
    private readonly IWorkflowEngine _workflowEngine;

    public HolidayService(
      DomainContext context,
      IWorkflowEngine workflowEngine
    )
    {
      _context = context;
      _workflowEngine = workflowEngine;
    }

    public HolidayViewModel GetNew()
    {
      var holiday = Holiday.Create("Me");
    
      _context.Add(holiday);
      _context.SaveChanges();

      return new HolidayViewModel
      {
        Id = holiday.Id,
        Requestor = holiday.Requestor,
        Superior = holiday.Superior
      };
    }

    public async Task<HolidayViewModel> GetAsync(int id)
    {
      var holiday = await _context.Holidays.FindAsync(id);

      return new HolidayViewModel
      {
        Id = holiday.Id,
        Requestor = holiday.Requestor,
        Superior = holiday.Superior,
        From = holiday.From,
        To = holiday.To
      };
    }
  }
}