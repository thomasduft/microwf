using System.Threading.Tasks;
using WebApi.Domain;
using WebApi.Models;

namespace WebApi.Services
{
  public interface IHolidayService
  {
    HolidayViewModel GetNew();
    Task<HolidayViewModel> GetAsync(int id);
  }

  public class HolidayService : IHolidayService
  {
    private readonly DomainContext _context;
    private readonly IWorkflowService _service;

    public HolidayService(
      DomainContext context,
      IWorkflowService service
    )
    {
      _context = context;
      _service = service;
    }

    public HolidayViewModel GetNew()
    {
      var holiday = new Holiday();
      holiday.Requestor = "Me";
      holiday.Superior = "NiceBoss";

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