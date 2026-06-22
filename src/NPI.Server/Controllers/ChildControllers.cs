using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NPI.Data.Entities;
using NPI.Data.UnitOfWork;
using NPI.Shared.DTOs;
using NPI.Shared.Enums;

namespace NPI.Server.Controllers;

// ══════════════════════════════════════════════════════
// Setup Questions Controller
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/npirecords/{npiId:int}/setup-questions")]
public class SetupQuestionsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public SetupQuestionsController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SetupQuestionDto>>>> GetAll(int npiId)
    {
        var items = await _uow.SetupQuestions.FindAsync(q => q.NpiRecordId == npiId);
        return Ok(new ApiResponse<List<SetupQuestionDto>> { Success = true, Data = _mapper.Map<List<SetupQuestionDto>>(items) });
    }

    [HttpPut("{id:int}/toggle")]
    public async Task<ActionResult<ApiResponse<SetupQuestionDto>>> Toggle(int npiId, int id, [FromBody] ToggleRequest req)
    {
        var entity = await _uow.SetupQuestions.GetByIdAsync(id);
        if (entity == null || entity.NpiRecordId != npiId)
            return NotFound(new ApiResponse<SetupQuestionDto> { Success = false, Message = "Question not found" });

        var oldValue = entity.Value;
        entity.Value = req.Value;
        entity.AnsweredBy = req.AnsweredBy;
        entity.AnsweredDate = DateTime.UtcNow;

        _uow.SetupQuestions.Update(entity);

        // Log change
        await _uow.ChangeLog.AddAsync(new ChangeLogEntry
        {
            NpiRecordId = npiId,
            FieldChanged = $"Setup.{entity.QuestionKey}",
            OldValue = oldValue.ToString(),
            NewValue = req.Value.ToString(),
            ChangedBy = req.AnsweredBy,
            ChangedDate = DateTime.UtcNow
        });

        await _uow.SaveChangesAsync();
        return Ok(new ApiResponse<SetupQuestionDto> { Success = true, Data = _mapper.Map<SetupQuestionDto>(entity) });
    }
}

// ══════════════════════════════════════════════════════
// Pilot Requirements Controller
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/npirecords/{npiId:int}/pilot-requirements")]
public class PilotRequirementsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public PilotRequirementsController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PilotRequirementDto>>>> GetAll(int npiId)
    {
        var items = await _uow.PilotRequirements.FindAsync(q => q.NpiRecordId == npiId);
        return Ok(new ApiResponse<List<PilotRequirementDto>> { Success = true, Data = _mapper.Map<List<PilotRequirementDto>>(items) });
    }

    [HttpPut("{id:int}/toggle")]
    public async Task<ActionResult<ApiResponse<PilotRequirementDto>>> Toggle(int npiId, int id, [FromBody] ToggleRequest req)
    {
        var entity = await _uow.PilotRequirements.GetByIdAsync(id);
        if (entity == null || entity.NpiRecordId != npiId)
            return NotFound(new ApiResponse<PilotRequirementDto> { Success = false, Message = "Not found" });
        var oldValue = entity.Value;
        entity.Value = req.Value;
        entity.AnsweredBy = req.AnsweredBy;
        entity.AnsweredDate = DateTime.UtcNow;
        _uow.PilotRequirements.Update(entity);

        // Log change
        await _uow.ChangeLog.AddAsync(new ChangeLogEntry
        {
            NpiRecordId = npiId,
            FieldChanged = $"Piolot.{entity.QuestionKey}",
            OldValue = oldValue.ToString(),
            NewValue = req.Value.ToString(),
            ChangedBy = req.AnsweredBy,
            ChangedDate = DateTime.UtcNow
        });
        await _uow.SaveChangesAsync();

        return Ok(new ApiResponse<PilotRequirementDto> { Success = true, Data = _mapper.Map<PilotRequirementDto>(entity) });
    }

    [HttpPost("{id:int}/insert")]
    public async Task<ActionResult<ApiResponse<string>>> Insert(int npiId, [FromBody] List<PilotRequirementDto> request)
    {
        foreach (var req in request)
        {
            var requirement = new PilotRequirement
            {
                NpiRecordId = req.NpiRecordId,
                QuestionKey = req.QuestionKey ?? "User",
                QuestionText = req.QuestionText ?? "User-added requirement",
                Note = req.Note,
                Value = req.Value,
                AnsweredBy = req.AnsweredBy,
                AnsweredDate = DateTime.UtcNow
            };
            await _uow.PilotRequirements.AddAsync(requirement);
        }
        await _uow.SaveChangesAsync();
        return Ok(new ApiResponse<string> { Success = true, Data = "Items Inserted" });
    }

    [HttpPost("{id:int}/delete")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int npiId)
    {
        var items = await _uow.PilotRequirements.FindAsync(x => x.Id == 31 || x.Id == 32 || x.Id == 33);
        foreach (var item in items)
        {
            _uow.PilotRequirements.Remove(item);
        }
        await _uow.SaveChangesAsync();
        return Ok(new ApiResponse<string> { Success = true, Data = "Items Deleted" });
    }
}

// ══════════════════════════════════════════════════════
// Planner Questions Controller
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/npirecords/{npiId:int}/planner-questions")]
public class PlannerQuestionsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public PlannerQuestionsController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PlannerQuestionDto>>>> GetAll(int npiId)
    {
        var items = await _uow.PlannerQuestions.FindAsync(q => q.NpiRecordId == npiId);
        return Ok(new ApiResponse<List<PlannerQuestionDto>> { Success = true, Data = _mapper.Map<List<PlannerQuestionDto>>(items) });
    }

    [HttpPut("{id:int}/toggle")]
    public async Task<ActionResult<ApiResponse<PlannerQuestionDto>>> Toggle(int npiId, int id, [FromBody] ToggleRequest req)
    {
        var entity = await _uow.PlannerQuestions.GetByIdAsync(id);
        if (entity == null || entity.NpiRecordId != npiId)
            return NotFound(new ApiResponse<PlannerQuestionDto> { Success = false, Message = "Not found" });
        var oldValue = entity.Value;
        entity.Value = req.Value;
        entity.AnsweredBy = req.AnsweredBy;
        entity.AnsweredDate = DateTime.UtcNow;
        _uow.PlannerQuestions.Update(entity);

        // Log change
        await _uow.ChangeLog.AddAsync(new ChangeLogEntry
        {
            NpiRecordId = npiId,
            FieldChanged = $"Piolot.{entity.QuestionKey}",
            OldValue = oldValue.ToString(),
            NewValue = req.Value.ToString(),
            ChangedBy = req.AnsweredBy,
            ChangedDate = DateTime.UtcNow
        });
        await _uow.SaveChangesAsync();

        await _uow.SaveChangesAsync();

        return Ok(new ApiResponse<PlannerQuestionDto> { Success = true, Data = _mapper.Map<PlannerQuestionDto>(entity) });
    }
    [HttpPost("{id:int}/insert")]
    public async Task<ActionResult<ApiResponse<PlannerQuestionDto>>> Insert(int npiId, [FromBody] ToggleRequest req)
    {
        var question = new PlannerQuestion
        {
            NpiRecordId = npiId,
            QuestionKey = req.AnsweredBy, // Using AnsweredBy to pass the question key for simplicity
            Value = req.Value,
            AnsweredBy = req.AnsweredBy,
            AnsweredDate = DateTime.UtcNow
        };
        await _uow.SaveChangesAsync();

        return Ok(new ApiResponse<PlannerQuestionDto> { Success = true, Data = _mapper.Map<PlannerQuestionDto>(question) });
    }
    [HttpPost("{id:int}/bulk-insert")]
    public async Task<ActionResult<ApiResponse<string>>> InsertBulk(int npiId, [FromBody] List<PlannerQuestionDto> request)
    {
        foreach (var req in request)
        {
            var question = new PlannerQuestion
            {
                NpiRecordId = req.NpiRecordId,
                QuestionText = req.QuestionText ?? "User-added question",
                Category = req.Category,
                QuestionKey = req.QuestionKey ?? "User", // Using AnsweredBy to pass the question key for simplicity
                Value = true,
                AnsweredBy = "User",
                AnsweredDate = DateTime.UtcNow
            };
            await _uow.PlannerQuestions.AddAsync(question);
        }
        await _uow.SaveChangesAsync();

        return Ok(new ApiResponse<string> { Success = true, Data = "Items inserted" });
    }

}

// ══════════════════════════════════════════════════════
// Packaging Components Controller
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/npirecords/{npiId:int}/packaging-components")]
public class PackagingComponentsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public PackagingComponentsController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PackagingComponentDto>>>> GetAll(int npiId)
    {
        var items = await _uow.PackagingComponents.FindAsync(
            predicate: c => c.NpiRecordId == npiId,
            orderBy: q => q.OrderBy(c => c.SortOrder));
        return Ok(new ApiResponse<List<PackagingComponentDto>> { Success = true, Data = _mapper.Map<List<PackagingComponentDto>>(items) });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PackagingComponentDto>>> Add(int npiId, [FromBody] PackagingComponentDto dto)
    {
        var entity = _mapper.Map<PackagingComponent>(dto);
        entity.NpiRecordId = npiId;
        await _uow.PackagingComponents.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Ok(new ApiResponse<PackagingComponentDto> { Success = true, Data = _mapper.Map<PackagingComponentDto>(entity) });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<PackagingComponentDto>>> Update(int npiId, int id, [FromBody] PackagingComponentDto dto)
    {
        var entity = await _uow.PackagingComponents.GetByIdAsync(id);
        if (entity == null || entity.NpiRecordId != npiId)
            return NotFound(new ApiResponse<PackagingComponentDto> { Success = false, Message = "Not found" });

        entity.ComponentType = dto.ComponentType;
        entity.ItemCode = dto.ItemCode;
        entity.Description = dto.Description;
        entity.Qty = dto.Qty;
        entity.UnitOfMeasure = dto.UnitOfMeasure;
        entity.IsCustomerSupplied = dto.IsCustomerSupplied;
        _uow.PackagingComponents.Update(entity);
        await _uow.SaveChangesAsync();

        return Ok(new ApiResponse<PackagingComponentDto> { Success = true, Data = _mapper.Map<PackagingComponentDto>(entity) });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int npiId, int id)
    {
        var entity = await _uow.PackagingComponents.GetByIdAsync(id);
        if (entity == null || entity.NpiRecordId != npiId)
            return NotFound(new ApiResponse<bool> { Success = false, Message = "Not found" });

        _uow.PackagingComponents.Remove(entity);
        await _uow.SaveChangesAsync();
        return Ok(new ApiResponse<bool> { Success = true, Data = true });
    }
}

// ══════════════════════════════════════════════════════
// Notes Controller
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/npirecords/{npiId:int}/notes")]
public class NotesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public NotesController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NpiNoteDto>>>> GetAll(int npiId)
    {
        var items = await _uow.Notes.FindAsync(
            predicate: n => n.NpiRecordId == npiId,
            orderBy: q => q.OrderByDescending(n => n.CreatedDate));
        return Ok(new ApiResponse<List<NpiNoteDto>> { Success = true, Data = _mapper.Map<List<NpiNoteDto>>(items) });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<NpiNoteDto>>> Add(int npiId, [FromBody] NpiNoteDto dto)
    {
        var entity = new NpiNote
        {
            NpiRecordId = npiId,
            Content = dto.Content,
            Parent = dto.Parent,
            CreatedBy = dto.CreatedBy,
            CreatedDate = DateTime.UtcNow
        };
        await _uow.Notes.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Ok(new ApiResponse<NpiNoteDto> { Success = true, Data = _mapper.Map<NpiNoteDto>(entity) });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int npiId, int id)
    {
        var entity = await _uow.Notes.GetByIdAsync(id);
        if (entity == null || entity.NpiRecordId != npiId)
            return NotFound(new ApiResponse<bool> { Success = false, Message = "Not found" });
        _uow.Notes.Remove(entity);
        await _uow.SaveChangesAsync();
        return Ok(new ApiResponse<bool> { Success = true, Data = true });
    }
}

// ══════════════════════════════════════════════════════
// Change Log Controller
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/npirecords/{npiId:int}/changelog")]
public class ChangeLogController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public ChangeLogController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ChangeLogEntryDto>>>> GetAll(int npiId)
    {
        var items = await _uow.ChangeLog.FindAsync(
            predicate: c => c.NpiRecordId == npiId,
            orderBy: q => q.OrderByDescending(c => c.ChangedDate));
        return Ok(new ApiResponse<List<ChangeLogEntryDto>> { Success = true, Data = _mapper.Map<List<ChangeLogEntryDto>>(items) });
    }
}


// ══════════════════════════════════════════════════════
// Change Log Controller
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/npirecords/{npiId:int}/itemcodesaxbom")]
public class ItemCodesAXBomController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public ItemCodesAXBomController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<List<ChangeLogEntryDto>>>> UpdateItem(int npiId, [FromBody] ItemCodeBomDto dto)
    {
        var entity = await _uow.NpiRecords.GetByIdAsync(npiId);
        if (entity == null || entity.Id != npiId)
            return NotFound(new ApiResponse<List<ChangeLogEntryDto>> { Success = false, Message = "Not found" });
        entity.ItemDescription = dto.ItemDesc;
        entity.CustomerName = dto.Customer;
        entity.BulkCode = dto.BulkCode;
        entity.Barcode = dto.BarCode;

        _uow.NpiRecords.Update(entity);
        await _uow.SaveChangesAsync();
        return Ok(new ApiResponse<List<ChangeLogEntryDto>> { Success = true, Data = new List<ChangeLogEntryDto>() });
    }

}


// ══════════════════════════════════════════════════════
// Update Questions
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/updatequestionsfigma")]
public class UpdateQuestionsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public UpdateQuestionsController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpPost]
    public async Task<ActionResult<string>> UpdateQuestions()
    {
        // setup questions 

        var plannerQuestionsToUpdate = await _uow.PlannerQuestions.FindAsync(q =>
           q.QuestionKey == "newrawmaterials"
        || q.QuestionKey == "customersupplied"
        || q.QuestionKey == "fromnewvendor"
        || q.QuestionKey == "newcomponents"
        || q.QuestionKey == "regulatorycerts"
        || q.QuestionKey == "newartwork"
        );
        foreach (var question in plannerQuestionsToUpdate)
        {
            if (question.QuestionKey == "customersupplied")
            {
                question.QuestionText = question.Category == PlannerCategory.Components ? "Are packaging components customer-supplied?"
                                                                                        : "Are any raw materials customer-supplied?";
                _uow.PlannerQuestions.Update(question);
            }
            else if (question.QuestionKey == "fromnewvendor")
            {
                question.QuestionText = question.Category == PlannerCategory.Components ? "Are packaging components sourced from a new vendor?"
                                                                                        : "Are raw materials sourced from a new vendor?";
                _uow.PlannerQuestions.Update(question);
            }
            else if (question.QuestionKey == "newcomponents")
            {
                question.QuestionText = "Does the packout include new components (excluding label)?";
                _uow.PlannerQuestions.Update(question);
            }
            else if (question.QuestionKey == "newrawmaterials")
            {
                question.QuestionText = "Does the formula include new raw materials?";
                _uow.PlannerQuestions.Update(question);
            }
            else if (question.QuestionKey == "regulatorycerts")
            {
                question.QuestionText = "Does this product require regulatory certifications or approvals?";
                _uow.PlannerQuestions.Update(question);
            }
            else if (question.QuestionKey == "newartwork")
            {
                question.QuestionText = "Does the packout require new or updated artwork?";
                _uow.PlannerQuestions.Update(question);
            }
        }

        await _uow.SaveChangesAsync();
        return Ok("Question updated as per new figma");
    }


}

[ApiController]
[Route("api/formulaspecs")]
public class FormulaSpecController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public FormulaSpecController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpPost]
    public async Task<ActionResult<string>> UpdateQuestions(FormulaSpecDto request)
    {
        var entity = new FormulaSpec
        {
            NpiRecordId = request.NpiRecordId,
            FormulaName = request.FormulaName,
            ProdType = request.ProdType,
            PkgType = request.PkgType,
            OrderQty = request.OrderQty,
            TotalUnits = request.TotalUnits,
            ServPerUnit = request.ServPerUnit,
            FormulaSize = request.FormulaSize,
            TotalServings = request.TotalServings,
            KG = request.KG,
            Density = request.Density,
            Blender = request.Blender,
            Batches = request.Batches
        };
        await _uow.FormulaSpecs.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return Ok("Formula Spec inserted");
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> InsertBulk(List<FormulaSpecDto> requests)
    {
        if (requests == null || !requests.Any())
            return BadRequest("No data provided");

        var entities = requests.Select(x => new FormulaSpec
        {
            NpiRecordId = x.NpiRecordId,
            FormulaName = x.FormulaName,
            ProdType = x.ProdType,
            PkgType = x.PkgType,
            OrderQty = x.OrderQty,
            TotalUnits = x.TotalUnits,
            ServPerUnit = x.ServPerUnit,
            FormulaSize = x.FormulaSize,
            TotalServings = x.TotalServings,
            KG = x.KG,
            Density = x.Density,
            Blender = x.Blender,
            Batches = x.Batches
        }).ToList();

        await _uow.FormulaSpecs.AddRangeAsync(entities);
        await _uow.SaveChangesAsync();

        return Ok($"{requests.Count} Formula Specs inserted successfully");
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAll()
    {
        var allSpecs = await _uow.FormulaSpecs.GetAllAsync();

        if (allSpecs == null || !allSpecs.Any())
            return Ok("No records found");

        _uow.FormulaSpecs.RemoveRange(allSpecs);
        await _uow.SaveChangesAsync();

        return Ok("All Formula Specs deleted successfully");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(int id, [FromBody] FormulaSpecDto request)
    {
        if (id != request.Id)
            return BadRequest("Id mismatch");

        var entity = await _uow.FormulaSpecs.GetByIdAsync(id);
        if (entity == null)
            return NotFound(false);

        // Update fields
        entity.FormulaName = request.FormulaName;
        entity.ProdType = request.ProdType;
        entity.PkgType = request.PkgType;
        entity.OrderQty = request.OrderQty;
        entity.TotalUnits = request.TotalUnits;
        entity.ServPerUnit = request.ServPerUnit;
        entity.FormulaSize = request.FormulaSize;
        entity.TotalServings = request.TotalServings;
        entity.KG = request.KG;
        entity.Density = request.Density;
        entity.Blender = request.Blender;
        entity.Batches = request.Batches;
        entity.NpiRecordId = request.NpiRecordId; // don't forget this if editable

        await _uow.SaveChangesAsync();

        return Ok(new ApiResponse<bool> { Success = true, Data = true, Message = "Updated" });
    }

}

[ApiController]
[Route("api/fggssetup")]
public class FGGSController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public FGGSController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> InsertOrUpdate(int npiId,FGGSSetupDto request)
    {
        var response = new ApiResponse<string>();

        // Check existing by NpiRecordId
        var entity = await _uow.FGSSSetup
            .FirstOrDefaultAsync(x => x.NpiRecordId == request.NpiRecordId);

        if (entity == null)
        {
            // INSERT
            entity = new FGGSSetup
            {
                NpiRecordId = npiId,
                DosageForm = request.DosageForm,
                ShapeAndColor = request.ShapeAndColor,
                TabletWeight = request.TabletWeight,
                Dimension = request.Dimension,
                CoatingType = request.CoatingType,
                Flavor = request.Flavor,
                PackageType = request.PackageType,
                CountPerUnit = request.CountPerUnit,
                ContainerMaterial = request.ContainerMaterial,
                ClouserType = request.ClouserType,
                SecondaryPackaging = request.SecondaryPackaging,
                LabelVersion = request.LabelVersion,
                RegulatoryMarket = request.RegulatoryMarket,
                TargetShelfLife = request.TargetShelfLife,
                StorageConditions = request.StorageConditions,
                SpecialHandling = request.SpecialHandling
            };

            await _uow.FGSSSetup.AddAsync(entity);
            response.Message = "FGGS Setup inserted successfully";
        }
        else
        {
            // UPDATE
            _mapper.Map(request, entity);
            _uow.FGSSSetup.Update(entity);
            response.Message = "FGGS Setup updated successfully";
        }

        await _uow.SaveChangesAsync();

        response.Success = true;
        return Ok(response);
    }

}


[ApiController]
[Route("api/qaquestion")]
public class QAPakcageQuestionController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public QAPakcageQuestionController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    [HttpPost("bulk-insert")]
    public async Task<ActionResult<string>> InsertQuestions()
    {
        var npiRecords = await _uow.NpiRecords.GetAllAsync();
        foreach (var record in npiRecords)
        {
            var questionList = new List<QualityPackageQuestion> {
                new QualityPackageQuestion
                {
                    NpiRecordId = record.Id,
                    QuestionText = "COA Required at Release",
                    Category = QAQuestionCategory.Package,
                    QuestionKey = "coarequired",
                    Value = false,
                    AnsweredBy = "User",
                    AnsweredDate = DateTime.UtcNow
                 },
                 new QualityPackageQuestion
                {
                    NpiRecordId = record.Id,
                    QuestionText = "Batch Record Template Linked",
                    Category = QAQuestionCategory.Package,
                    QuestionKey = "batchrecordtemplate",
                    Value = false,
                    AnsweredBy = "User",
                    AnsweredDate = DateTime.UtcNow
                 }
               
            };
            await _uow.QualityPackageQuestion.AddRangeAsync(questionList);
        }

        await _uow.SaveChangesAsync();

        return Ok("QA Package questions inserted for all records");
    }

    [HttpPut("toggle")]
    public async Task<ActionResult<ApiResponse<QualityPackageQuestionDto>>> Toggle(int npiId, int id, [FromBody] ToggleRequest req)
    {
        var entity = await _uow.QualityPackageQuestion.GetByIdAsync(id);
        if (entity == null || entity.NpiRecordId != npiId)
            return NotFound(new ApiResponse<QualityPackageQuestionDto> { Success = false, Message = "Not found" });
        var oldValue = entity.Value;
        entity.Value = req.Value;
        entity.AnsweredBy = req.AnsweredBy;
        entity.AnsweredDate = DateTime.UtcNow;
        _uow.QualityPackageQuestion.Update(entity);

        // Log change
        await _uow.ChangeLog.AddAsync(new ChangeLogEntry
        {
            NpiRecordId = npiId,
            FieldChanged = $"FGSS.{entity.QuestionKey}",
            OldValue = oldValue.ToString(),
            NewValue = req.Value.ToString(),
            ChangedBy = req.AnsweredBy,
            ChangedDate = DateTime.UtcNow
        });
        await _uow.SaveChangesAsync();

        await _uow.SaveChangesAsync();

        return Ok(new ApiResponse<QualityPackageQuestionDto> { Success = true, Data = _mapper.Map<QualityPackageQuestionDto>(entity) });
    }
}


[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public UserController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // CREATE USER WITH ROLES
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest req)
    {
        var existing = (await _uow.Users.FindAsync(x => x.Email == req.Email)).FirstOrDefault();

        if (existing != null)
            return BadRequest("User already exists");

        var user = new User
        {
            UserName = req.UserName,
            Email = req.Email,
            Password = req.Password
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        var userRoles = req.RoleIds.Select(roleId => new UserRole
        {
            UserId = user.Id,
            RoleId = roleId
        }).ToList();

        await _uow.UserRoles.AddRangeAsync(userRoles);

        await _uow.SaveChangesAsync();

        return Ok("User created successfully");
    }

    // UPDATE USER BY EMAIL
    [HttpPut("update-by-email")]
    public async Task<IActionResult> UpdateUserByEmail(string email, [FromBody] UpdateUserByEmailRequest req)
    {
        var user = (await _uow.Users.FindAsync(x => x.Email == email)).FirstOrDefault();

        if (user == null)
            return NotFound("User not found");

        user.UserName = req.UserName;
        user.Email = req.Email;
        user.Password = req.Password;

        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();

        return Ok("User updated successfully");
    }
}