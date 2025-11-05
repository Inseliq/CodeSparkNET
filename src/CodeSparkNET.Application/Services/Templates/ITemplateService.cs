using CodeSparkNET.Application.Dtos.Templates;

namespace CodeSparkNET.Application.Services.Templates
{
    public interface ITemplateService
    {
        Task<TemplateDto> CreateTemplateAsync(AddTemplateDto model);
        Task<TemplateDto> GetTemplateByIdAsync(string id);
        Task<TemplateDto> GetTemplateBySlugAsync(string slug);
        Task<TemplateDto> GetTemplateByIdOrSlugAsync(string query);
        Task<IEnumerable<TemplateDto>> GetAllTemplatesAsync();
        Task<bool> UpdateTemplateAsync(UpdateTemplateDto model);
        Task<bool> DeleteTemplateByIdAsync(string id);
        Task<bool> DeleteTemplateBySlugAsync(string slug);
    }
}
