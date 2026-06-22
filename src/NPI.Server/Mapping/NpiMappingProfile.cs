using AutoMapper;
using NPI.Data.Entities;
using NPI.Shared.DTOs;

namespace NPI.Server.Mapping;

public class NpiMappingProfile : Profile
{
    public NpiMappingProfile()
    {
        // Entity → DTO
        CreateMap<NpiRecord, NpiRecordDto>().ReverseMap(); 
        CreateMap<SetupQuestion, SetupQuestionDto>().ReverseMap();
        CreateMap<PilotRequirement, PilotRequirementDto>().ReverseMap();
        CreateMap<PlannerQuestion, PlannerQuestionDto>().ReverseMap();
        CreateMap<QualityPackageQuestion, QualityPackageQuestionDto>().ReverseMap();
        CreateMap<FGGSSetup, FGGSSetupDto>().ReverseMap();
        CreateMap<FormulaSpec, FormulaSpecDto>().ReverseMap();
        CreateMap<LaborItem, LaborItemDto>().ReverseMap();
        CreateMap<PackagingComponent, PackagingComponentDto>().ReverseMap();
        CreateMap<PackagingOption, PackagingOptionDto>().ReverseMap();
        CreateMap<NpiDocument, NpiDocumentDto>().ReverseMap();
        CreateMap<NpiNote, NpiNoteDto>().ReverseMap();
        CreateMap<ChangeLogEntry, ChangeLogEntryDto>().ReverseMap();
    }
}
