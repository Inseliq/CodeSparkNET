using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Dtos;
using CodeSparkNET.Dtos.Catalog;

namespace CodeSparkNET.Interfaces
{
    public interface ICatalogInterface
    {
        Task<List<CatalogDto>> GetCatalogItemsAsync(int page, int size);
    }
}