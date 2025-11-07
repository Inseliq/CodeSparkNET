using CodeSparkNET.Application.Dtos.Course;
using CodeSparkNET.Application.Dtos.Templates;
using CodeSparkNET.Domain.Models;

namespace CodeSparkNET.Application.Services.Templates
{
    public class TemplateService : ITemplateService
    {
        private readonly IProductRepository _productRepository;
        public TemplateService(
            IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<TemplateDto> CreateTemplateAsync(AddTemplateDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var template = new Template
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Slug = model.Slug,
                ShortDescription = model.ShortDescription,
                FullDescription = model.FullDescription,
                Price = model.Price,
                Currency = model.Currency,
                IsPublished = model.IsPublished,
                InStock = model.InStock,
                CatalogId = model.CatalogId,
                ProductType = model.ProductType,
            };

            template.ProductImages = model.ProductImages.Select(pi => new ProductImage
            {
                Id = Guid.NewGuid().ToString(),
                Url = pi.Url,
                AltText = pi.AltText,
                IsMain = pi.IsMain,
                Product = template,
                ProductId = template.Id,
                Name = pi.Name
            }).ToList();

            var res = await _productRepository.CreateTemplateAsync(template);
            if (res == null)
                throw new Exception("Failed to create template.");
            return new TemplateDto
            {
                Name = res.Name,
                Slug = res.Slug,
                ShortDescription = res.ShortDescription,
                FullDescription = res.FullDescription,
                Price = res.Price,
                Currency = res.Currency,
                IsPublished = res.IsPublished,
                InStock = res.InStock,
                CatalogId = res.CatalogId,
                ProductType = res.ProductType,
                ProductImages = res.ProductImages.Select(pi => new ProductImageDto
                {
                    Url = pi.Url,
                    AltText = pi.AltText,
                    IsMain = pi.IsMain,
                    Name = pi.Name ?? ""
                }).ToList()
            };
        }

        public async Task<TemplateDto> GetTemplateByIdAsync(string id)
        {
            var template = await _productRepository.GetTemplateByIdAsync(id);
            if (template == null)
                return null;
            return new TemplateDto
            {
                Name = template.Name,
                Slug = template.Slug,
                ShortDescription = template.ShortDescription,
                FullDescription = template.FullDescription,
                Price = template.Price,
                Currency = template.Currency,
                IsPublished = template.IsPublished,
                InStock = template.InStock,
                CatalogId = template.CatalogId,
                ProductType = template.ProductType,
                ProductImages = template.ProductImages.Select(pi => new ProductImageDto
                {
                    Url = pi.Url,
                    AltText = pi.AltText,
                    IsMain = pi.IsMain,
                    Name = pi.Name ?? ""
                }).ToList()
            };
        }

        public async Task<TemplateDto> GetTemplateBySlugAsync(string slug)
        {
            var template = await _productRepository.GetTemplateBySlugAsync(slug);
            if (template == null)
                return null;
            return new TemplateDto
            {
                Name = template.Name,
                Slug = template.Slug,
                ShortDescription = template.ShortDescription,
                FullDescription = template.FullDescription,
                Price = template.Price,
                Currency = template.Currency,
                IsPublished = template.IsPublished,
                InStock = template.InStock,
                CatalogId = template.CatalogId,
                ProductType = template.ProductType,
                ProductImages = template.ProductImages.Select(pi => new ProductImageDto
                {
                    Url = pi.Url,
                    AltText = pi.AltText,
                    IsMain = pi.IsMain,
                    Name = pi.Name ?? ""
                }).ToList()
            };
        }

        public async Task<TemplateDto> GetTemplateByIdOrSlugAsync(string query)
        {
            var template = await GetTemplateByIdAsync(query);
            if (template != null)
                return template;
            return await GetTemplateBySlugAsync(query);
        }

        public async Task<IEnumerable<TemplateDto>> GetAllTemplatesAsync()
        {
            var templates = await _productRepository.GetAllTemplatesAsync();
            return templates.Select(template => new TemplateDto
            {
                Name = template.Name,
                Slug = template.Slug,
                ShortDescription = template.ShortDescription,
                FullDescription = template.FullDescription,
                Price = template.Price,
                Currency = template.Currency,
                IsPublished = template.IsPublished,
                InStock = template.InStock,
                CatalogId = template.CatalogId,
                ProductType = template.ProductType,
                ProductImages = template.ProductImages.Select(pi => new ProductImageDto
                {
                    Url = pi.Url,
                    AltText = pi.AltText,
                    IsMain = pi.IsMain,
                    Name = pi.Name ?? ""
                }).ToList()
            });
        }

        public async Task<bool> UpdateTemplateAsync(UpdateTemplateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Template Name is required", nameof(dto.Name));
            var currTemplate = await _productRepository.GetTemplateBySlugAsync(dto.Slug);
            if (currTemplate is null)
                return false;

            var template = new Template
            {
                Id = currTemplate.Id,
                Name = dto.Name,
                Slug = dto.Slug,
                Price = dto.Price,
                Currency = string.IsNullOrWhiteSpace(dto.Currency) ? "RUB" : dto.Currency,
                InStock = dto.InStock,
                ProductType = dto.ProductType,
                CatalogId = dto.CatalogId,
                ShortDescription = dto.ShortDescription,
                FullDescription = dto.FullDescription,
                IsPublished = dto.IsPublished
            };

            // Маппинг изображений вручную
            if (dto.ProductImages != null && dto.ProductImages.Any())
            {
                template.ProductImages = dto.ProductImages.Select((imgDto, index) => new ProductImage
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = imgDto.Name,
                    Url = imgDto.Url,
                    AltText = imgDto.AltText,
                    IsMain = imgDto.IsMain,
                    Position = index,
                    Product = currTemplate,
                    ProductId = currTemplate.Id
                }).ToList();
            }

            return await _productRepository.UpdateTemplateAsync(template);
        }

        public async Task<bool> DeleteTemplateByIdAsync(string id)
        {
            var existingTemplate = await _productRepository.GetTemplateByIdAsync(id);
            if (existingTemplate == null)
                throw new Exception("Template not found.");
            return await _productRepository.DeleteTemplateByIdAsync(id);
        }

        public async Task<bool> DeleteTemplateBySlugAsync(string slug)
        {
            var existingTemplate = await _productRepository.GetTemplateBySlugAsync(slug);
            if (existingTemplate == null)
                throw new Exception("Template not found.");
            return await _productRepository.DeleteTemplateBySlugAsync(slug);
        }
    }
}
