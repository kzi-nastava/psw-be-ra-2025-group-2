using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Explorer.Tours.Core.UseCases;
public class TourSaleService : ITourSaleService
{
    private readonly ITourSaleRepository _repo;
    private readonly IMapper _mapper;

    public TourSaleService(ITourSaleRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public TourSaleDto Create(TourSaleDto dto)
    {
        var sale = new TourSale(dto.AuthorId, dto.StartDate, dto.EndDate,
            dto.DiscountPercentage, dto.TourIds);

        return _mapper.Map<TourSaleDto>(_repo.Create(sale));
    }

    public TourSaleDto Update(long authorId, TourSaleDto dto)
    {
        var existing = _repo.GetById(dto.Id) ?? throw new ArgumentException("Not found");
        if (existing.AuthorId != authorId) throw new ArgumentException();

        existing.Update(dto.StartDate, dto.EndDate, dto.DiscountPercentage, dto.TourIds);
        return _mapper.Map<TourSaleDto>(_repo.Update(existing));
    }

    public void Delete(long authorId, long id)
    {
        var existing = _repo.GetById(id) ?? throw new ArgumentException();
        if (existing.AuthorId != authorId) throw new ArgumentException();
        _repo.Delete(id);
    }

    /*public PagedResult<TourSaleDto> GetPagedByAuthor(long authorId, int page, int pageSize)
        => _repo.GetPagedByAuthor(authorId, page, pageSize).Map(_mapper.Map<TourSaleDto>);

    public PagedResult<TourSaleDto> GetActive(int page, int pageSize)
    {
        return _repo.GetActive(page, pageSize).Map(_mapper.Map<TourSaleDto>);
    }*/

    public int GetPageCount(int pageSize)
        => (int)Math.Ceiling((double)_repo.GetCount() / pageSize);

    public int GetPageCountByAuthor(long authorId, int pageSize)
        => (int)Math.Ceiling((double)_repo.GetCountByAuthor(authorId) / pageSize);
}
