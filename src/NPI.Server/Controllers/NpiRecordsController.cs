using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPI.Data.Entities;
using NPI.Data.UnitOfWork;
using NPI.Shared.DTOs;
using NPI.Shared.Enums;

namespace NPI.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NpiRecordsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public NpiRecordsController(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    // ── GET all records ──
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NpiRecordDto>>>> GetAll([FromQuery] string? search, [FromQuery] int? status)
    {
        try
        {
            List<NpiRecord> records;

            if (!string.IsNullOrEmpty(search))
            {
                records = (await _uow.NpiRecords.SearchAsync(search, status: status.HasValue ? (NpiStatus)status : null, page: 1, pageSize: 100)).ToList();
            }
            else if (status.HasValue)
            {
                records = (await _uow.NpiRecords.GetByStatusAsync((NpiStatus)status)).ToList();
            }
            else
            {
                records = (List<NpiRecord>)await _uow.NpiRecords.FindAsync(orderBy: q => q.OrderByDescending(r => r.CreatedDate));
            }

            var dtos = _mapper.Map<List<NpiRecordDto>>(records);
            return Ok(new ApiResponse<List<NpiRecordDto>> { Success = true, Data = dtos });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<NpiRecordDto>> { Success = false, Message = ex.Message });
        }
    }
    [HttpGet("export")]
    public async Task<IActionResult> Export(
     [FromQuery] string? search,
     [FromQuery] int? status)
    {
        try
        {
            NpiStatus? statusEnum = status.HasValue
                ? (NpiStatus)status.Value
                : null;

            var fileBytes = await _uow.NpiRecords.ExportAsync(search, statusEnum);

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "BidsWon.xlsx");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    // ── GET by ID ──
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NpiRecordDto>>> GetById(int id)
    {
        try
        {
            var record = await _uow.NpiRecords.GetFullRecordAsync(id);
            if (record == null)
                return NotFound(new ApiResponse<NpiRecordDto> { Success = false, Message = "Record not found" });

            var dto = _mapper.Map<NpiRecordDto>(record);
            return Ok(new ApiResponse<NpiRecordDto> { Success = true, Data = dto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiRecordDto> { Success = false, Message = ex.Message });
        }
    }

    // ── GET setup data ──
    [HttpGet("{id}/setup")]
    public async Task<ActionResult<ApiResponse<NpiRecordDto>>> GetSetup(int id)
    {
        try
        {
            var record = await _uow.NpiRecords.GetByIdAsync(id,
                r => r.SetupQuestions,
                r => r.PilotRequirements,
                r => r.PlannerQuestions,
                r => r.FormulaSpec);

            if (record == null)
                return NotFound(new ApiResponse<NpiRecordDto> { Success = false, Message = "Record not found" });

            var dto = _mapper.Map<NpiRecordDto>(record);
            return Ok(new ApiResponse<NpiRecordDto> { Success = true, Data = dto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiRecordDto> { Success = false, Message = ex.Message });
        }
    }

    // ── GET BOM data ──
    [HttpGet("{id}/bom")]
    public async Task<ActionResult<ApiResponse<NpiRecordDto>>> GetBom(int id)
    {
        try
        {
            var record = await _uow.NpiRecords.GetByIdAsync(id, r => r.LaborItems);
            if (record == null)
                return NotFound(new ApiResponse<NpiRecordDto> { Success = false, Message = "Record not found" });

            var dto = _mapper.Map<NpiRecordDto>(record);
            return Ok(new ApiResponse<NpiRecordDto> { Success = true, Data = dto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiRecordDto> { Success = false, Message = ex.Message });
        }
    }

    // ── GET packaging data ──
    [HttpGet("{id}/packaging")]
    public async Task<ActionResult<ApiResponse<NpiRecordDto>>> GetPackaging(int id)
    {
        try
        {
            var record = await _uow.NpiRecords.GetByIdAsync(id,
                r => r.PackagingComponents,
                r => r.PackagingOptions);

            if (record == null)
                return NotFound(new ApiResponse<NpiRecordDto> { Success = false, Message = "Record not found" });

            var dto = _mapper.Map<NpiRecordDto>(record);
            return Ok(new ApiResponse<NpiRecordDto> { Success = true, Data = dto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiRecordDto> { Success = false, Message = ex.Message });
        }
    }

    // ── GET documents ──
    [HttpGet("{id}/docs")]
    public async Task<ActionResult<ApiResponse<NpiRecordDto>>> GetDocs(int id)
    {
        try
        {
            var record = await _uow.NpiRecords.GetByIdAsync(id, r => r.Documents);
            if (record == null)
                return NotFound(new ApiResponse<NpiRecordDto> { Success = false, Message = "Record not found" });

            var dto = _mapper.Map<NpiRecordDto>(record);
            return Ok(new ApiResponse<NpiRecordDto> { Success = true, Data = dto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiRecordDto> { Success = false, Message = ex.Message });
        }
    }

    // ── POST create ──
    [HttpPost]
    public async Task<ActionResult<ApiResponse<NpiRecordDto>>> Create([FromBody] NpiRecordDto dto)
    {
        try
        {
            var record = _mapper.Map<NpiRecord>(dto);
            await _uow.NpiRecords.AddAsync(record);
            await _uow.SaveChangesAsync();

            var resultDto = _mapper.Map<NpiRecordDto>(record);
            return CreatedAtAction(nameof(GetById), new { id = record.Id },
                new ApiResponse<NpiRecordDto> { Success = true, Data = resultDto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiRecordDto> { Success = false, Message = ex.Message });
        }
    }

    // ── PUT update ──
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<NpiRecordDto>>> Update(int id, [FromBody] NpiRecordDto dto)
    {
        try
        {
            var record = await _uow.NpiRecords.GetByIdAsync(id);
            if (record == null)
                return NotFound(new ApiResponse<NpiRecordDto> { Success = false, Message = "Record not found" });

            _mapper.Map(dto, record);
            _uow.NpiRecords.Update(record);
            await _uow.SaveChangesAsync();

            var resultDto = _mapper.Map<NpiRecordDto>(record);
            return Ok(new ApiResponse<NpiRecordDto> { Success = true, Data = resultDto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiRecordDto> { Success = false, Message = ex.Message });
        }
    }

    // ── DELETE ──
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var record = await _uow.NpiRecords.GetByIdAsync(id);
            if (record == null)
                return NotFound(new ApiResponse<bool> { Success = false, Message = "Record not found" });

            _uow.NpiRecords.Remove(record);
            await _uow.SaveChangesAsync();

            return Ok(new ApiResponse<bool> { Success = true, Data = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool> { Success = false, Message = ex.Message });
        }
    }

    // ── GET labor total ──
    [HttpGet("{id}/labor-total/{category}")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetLaborTotal(int id, LaborCategory category)
    {
        try
        {
            var total = await _uow.NpiRecords.GetTotalLaborCostAsync(id, category);
            return Ok(new ApiResponse<decimal> { Success = true, Data = total });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<decimal> { Success = false, Message = ex.Message });
        }
    }

    // ── Setup Questions Toggle ──
    [HttpPut("{npiId}/setup-questions/{questionId}/toggle")]
    public async Task<ActionResult<ApiResponse<SetupQuestionDto>>> ToggleSetupQuestion(int npiId, int questionId, [FromBody] ToggleRequest req)
    {
        try
        {
            var question = await _uow.SetupQuestions.GetByIdAsync(questionId);
            if (question == null)
                return NotFound(new ApiResponse<SetupQuestionDto> { Success = false, Message = "Question not found" });

            question.Value = req.Value;
            question.AnsweredBy = req.AnsweredBy;
            question.AnsweredDate = DateTime.UtcNow;
            _uow.SetupQuestions.Update(question);
            await _uow.SaveChangesAsync();

            var dto = _mapper.Map<SetupQuestionDto>(question);
            return Ok(new ApiResponse<SetupQuestionDto> { Success = true, Data = dto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<SetupQuestionDto> { Success = false, Message = ex.Message });
        }
    }

    // ── Pilot Requirements Toggle ──
    [HttpPut("{npiId}/pilot-requirements/{reqId}/toggle")]
    public async Task<ActionResult<ApiResponse<PilotRequirementDto>>> TogglePilot(int npiId, int reqId, [FromBody] ToggleRequest req)
    {
        try
        {
            var requirement = await _uow.PilotRequirements.GetByIdAsync(reqId);
            if (requirement == null)
                return NotFound(new ApiResponse<PilotRequirementDto> { Success = false, Message = "Requirement not found" });

            requirement.Value = req.Value;
            requirement.AnsweredBy = req.AnsweredBy;
            requirement.AnsweredDate = DateTime.UtcNow;
            _uow.PilotRequirements.Update(requirement);
            await _uow.SaveChangesAsync();

            var dto = _mapper.Map<PilotRequirementDto>(requirement);
            return Ok(new ApiResponse<PilotRequirementDto> { Success = true, Data = dto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<PilotRequirementDto> { Success = false, Message = ex.Message });
        }
    }

    // ── Planner Questions Toggle ──
    [HttpPut("{npiId}/planner-questions/{qId}/toggle")]
    public async Task<ActionResult<ApiResponse<PlannerQuestionDto>>> TogglePlanner(int npiId, int qId, [FromBody] ToggleRequest req)
    {
        try
        {
            var question = await _uow.PlannerQuestions.GetByIdAsync(qId);
            if (question == null)
                return NotFound(new ApiResponse<PlannerQuestionDto> { Success = false, Message = "Question not found" });

            question.Value = req.Value;
            _uow.PlannerQuestions.Update(question);
            await _uow.SaveChangesAsync();

            var dto = _mapper.Map<PlannerQuestionDto>(question);
            return Ok(new ApiResponse<PlannerQuestionDto> { Success = true, Data = dto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<PlannerQuestionDto> { Success = false, Message = ex.Message });
        }
    }

    // ── Packaging Components ──
    [HttpGet("{npiId}/packaging-components")]
    public async Task<ActionResult<ApiResponse<List<PackagingComponentDto>>>> GetPackagingComponents(int npiId)
    {
        try
        {
            var components = await _uow.PackagingComponents.FindAsync(c => c.NpiRecordId == npiId);
            var dtos = _mapper.Map<List<PackagingComponentDto>>(components);
            return Ok(new ApiResponse<List<PackagingComponentDto>> { Success = true, Data = dtos });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<PackagingComponentDto>> { Success = false, Message = ex.Message });
        }
    }

    [HttpPost("{npiId}/packaging-components")]
    public async Task<ActionResult<ApiResponse<PackagingComponentDto>>> AddPackagingComponent(int npiId, [FromBody] PackagingComponentDto dto)
    {
        try
        {
            var component = _mapper.Map<PackagingComponent>(dto);
            component.NpiRecordId = npiId;
            await _uow.PackagingComponents.AddAsync(component);
            await _uow.SaveChangesAsync();

            var resultDto = _mapper.Map<PackagingComponentDto>(component);
            return Ok(new ApiResponse<PackagingComponentDto> { Success = true, Data = resultDto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<PackagingComponentDto> { Success = false, Message = ex.Message });
        }
    }

    [HttpPut("{npiId}/packaging-components/{id}")]
    public async Task<ActionResult<ApiResponse<PackagingComponentDto>>> UpdatePackagingComponent(int npiId, int id, [FromBody] PackagingComponentDto dto)
    {
        try
        {
            var component = await _uow.PackagingComponents.GetByIdAsync(id);
            if (component == null)
                return NotFound(new ApiResponse<PackagingComponentDto> { Success = false, Message = "Component not found" });

            _mapper.Map(dto, component);
            _uow.PackagingComponents.Update(component);
            await _uow.SaveChangesAsync();

            var resultDto = _mapper.Map<PackagingComponentDto>(component);
            return Ok(new ApiResponse<PackagingComponentDto> { Success = true, Data = resultDto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<PackagingComponentDto> { Success = false, Message = ex.Message });
        }
    }

    [HttpDelete("{npiId}/packaging-components/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePackagingComponent(int npiId, int id)
    {
        try
        {
            var component = await _uow.PackagingComponents.GetByIdAsync(id);
            if (component == null)
                return NotFound(new ApiResponse<bool> { Success = false, Message = "Component not found" });

            _uow.PackagingComponents.Remove(component);
            await _uow.SaveChangesAsync();

            return Ok(new ApiResponse<bool> { Success = true, Data = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool> { Success = false, Message = ex.Message });
        }
    }

    // ── Notes ──
    [HttpGet("{npiId}/notes")]
    public async Task<ActionResult<ApiResponse<List<NpiNoteDto>>>> GetNotes(int npiId)
    {
        try
        {
            var notes = await _uow.Notes.FindAsync(
                         n => n.NpiRecordId == npiId,
                         orderBy: q => q.OrderByDescending(n => n.CreatedDate)
                     );

            if (notes == null || !notes.Any())
            {
                return Ok(new ApiResponse<List<NpiNoteDto>>
                {
                    Success = true,
                    Data = new List<NpiNoteDto>()
                });
            }

            var filtered = notes.Where(x => x.Parent == NotesParent.Notes);
            var dtos = _mapper.Map<List<NpiNoteDto>>(filtered);

            return Ok(new ApiResponse<List<NpiNoteDto>>
            {
                Success = true,
                Data = dtos
            });

        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<NpiNoteDto>> { Success = false, Message = ex.Message });
        }
    }

    [HttpPost("{npiId}/notes")]
    public async Task<ActionResult<ApiResponse<NpiNoteDto>>> AddNote(int npiId, [FromBody] NpiNoteDto dto)
    {
        try
        {
            var note = _mapper.Map<NpiNote>(dto);
            note.NpiRecordId = npiId;
            note.CreatedDate = DateTime.UtcNow;
            await _uow.Notes.AddAsync(note);
            await _uow.SaveChangesAsync();

            var resultDto = _mapper.Map<NpiNoteDto>(note);
            return Ok(new ApiResponse<NpiNoteDto> { Success = true, Data = resultDto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiNoteDto> { Success = false, Message = ex.Message });
        }
    }

    [HttpDelete("{npiId}/notes/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteNote(int npiId, int id)
    {
        try
        {
            var note = await _uow.Notes.GetByIdAsync(id);
            if (note == null)
                return NotFound(new ApiResponse<bool> { Success = false, Message = "Note not found" });

            _uow.Notes.Remove(note);
            await _uow.SaveChangesAsync();

            return Ok(new ApiResponse<bool> { Success = true, Data = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool> { Success = false, Message = ex.Message });
        }
    }

    // ── Change Log ──
    [HttpGet("{npiId}/changelog")]
    public async Task<ActionResult<ApiResponse<List<ChangeLogEntryDto>>>> GetChangeLog(int npiId)
    {
        try
        {
            var entries = await _uow.ChangeLog.FindAsync(c => c.NpiRecordId == npiId, orderBy: q => q.OrderByDescending(c => c.ChangedDate));
            var dtos = _mapper.Map<List<ChangeLogEntryDto>>(entries);
            return Ok(new ApiResponse<List<ChangeLogEntryDto>> { Success = true, Data = dtos });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<ChangeLogEntryDto>> { Success = false, Message = ex.Message });
        }
    }

    // inserts

    [HttpPost("{npiId}/insertsetupquestion")]
    public async Task<ActionResult<ApiResponse<SetupQuestionDto>>> InsertSetupQuestion(int npiId, [FromBody] SetupQuestionDto dto)
    {
        try
        {
            var question = new SetupQuestion
            {
                Note = dto.Note,
                QuestionKey = dto.QuestionKey,
                QuestionText = dto.QuestionText,
                Value = dto.Value,
                AnsweredBy = dto.AnsweredBy,
                AnsweredDate = dto.AnsweredDate,
                NpiRecordId = dto.NpiRecordId,

            };
            await _uow.SetupQuestions.AddAsync(question);
            await _uow.SaveChangesAsync();

            var resultDto = _mapper.Map<SetupQuestionDto>(question);
            return Ok(new ApiResponse<SetupQuestionDto> { Success = true, Data = resultDto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiNoteDto> { Success = false, Message = ex.Message });
        }
    }

    [HttpPost("{npiId}/updatesetupquestion")]
    public async Task<ActionResult<ApiResponse<SetupQuestionDto>>> UpdateSetupQuestion(int npiId, [FromBody] SetupQuestionDto dto)
    {
        try
        {
            var entity = await _uow.SetupQuestions.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity == null)
                return NotFound("SetupQuestion not found");
            entity.NpiRecordId = dto.NpiRecordId;
            _uow.SetupQuestions.Update(entity);
            await _uow.SaveChangesAsync();

            var resultDto = _mapper.Map<SetupQuestionDto>(entity);
            return Ok(new ApiResponse<SetupQuestionDto> { Success = true, Data = resultDto });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<NpiNoteDto> { Success = false, Message = ex.Message });
        }
    }


    // create default questions and labor items when a new NPI record is created
    [HttpPost("insertnpirecord")]
    public async Task<IActionResult> CreateNpiRecord([FromBody] CreateNpiRecordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var npiRecord = new NpiRecord
        {
            ItemCode = dto.ItemCode,
            ProductDescription = dto.ProductDescription,
            QuoteNumber = dto.QuoteNumber,

            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName,
            SalesRep1 = dto.SalesRep1,
            SalesRep2 = dto.SalesRep2,

            BulkCode = dto.BulkCode,
            FGCode = dto.FGCode,
            PackoutDescription = dto.PackoutDescription,

            ProductType = dto.ProductType,
            PackageType = dto.PackageType,

            OrderQty = dto.OrderQty,
            TotalUnits = dto.TotalUnits,
            FormulaSize = dto.FormulaSize,
            Servings = dto.Servings,
            UnitPrice = dto.UnitPrice,
            StdQty = dto.StdQty,
            LeadTimeDays = dto.LeadTimeDays,
            POValue = dto.POValue,

            ItemDescription = dto.ItemDescription,
            CustomerNumber = dto.CustomerNumber,
            Barcode = dto.Barcode,

            Status = dto.Status.HasValue ? (NpiStatus)dto.Status.Value : NpiStatus.Draft
        };
        npiRecord.SetupQuestions = new List<SetupQuestion>
    {
        new()
        {
            QuestionKey = "existingBulk",
            QuestionText = "Does this new product use an existing bulk?",
            Value = true
        },
        new()
        {
            QuestionKey = "multiplePacks",
            QuestionText = "Will this bulk be used in multiple packouts?",
            Value = true
        },
        new()
        {
            QuestionKey = "bulkFinished",
            QuestionText = "Will this item be sold as a Bulk Finished Good?",
            Value = true
        }
    };
        npiRecord.LaborItems = new List<LaborItem>
    {
        // ───── FORMULA LABOR ─────
        new()
        {
            Category = LaborCategory.Formula,
            IsOverhead = false,
            Description = "Weigh-Up",
            Base = "Batch",
            Rate = 0,
            Cost = 0,
            SortOrder = 1
        },
        new()
        {
            Category = LaborCategory.Formula,
            IsOverhead = false,
            Description = "Blending / Mixing",
            Base = "Batch",
            Rate = 0,
            Cost = 0,
            SortOrder = 2
        },

        // ───── PACKOUT LABOR ─────
        new()
        {
            Category = LaborCategory.Packout,
            IsOverhead = false,
            Description = "Bottle Filling",
            Base = "Per Hour",
            UnitsPerHour = 0,
            Rate = 0,
            Cost = 0,
            SortOrder = 3
        },
        new()
        {
            Category = LaborCategory.Packout,
            IsOverhead = false,
            Description = "Labeling & Packing",
            Base = "Per Hour",
            UnitsPerHour = 0,
            Rate = 0,
            Cost = 0,
            SortOrder = 4
        },


    };
        npiRecord.PlannerQuestions = new List<PlannerQuestion>
        {
            new()
            {
                Category = PlannerCategory.Components,
                QuestionKey = "newcomponents",
                Value= true,
                QuestionText = "New components?"
            },
            new()
            {
                Category = PlannerCategory.Components,
                QuestionKey = "customersupplied",
                Value= true,
                QuestionText = "Customer-supplied?"
            },
            new()
            {
                Category = PlannerCategory.Components,
                QuestionKey = "fromnewvendor",
                Value= true,
                QuestionText = "From new vendor?"
            },
            new()
            {
                Category = PlannerCategory.Components,
                QuestionKey = "newartwork",
                Value= true,
                QuestionText = "New artwork?"
            },
            new()
            {
                Category = PlannerCategory.Components,
                Value= true,
                QuestionKey = "regulatorycerts",
                QuestionText = "Regulatory certs?"
            },
            // Raw Materials
             new()
            {
                Category = PlannerCategory.RawMaterials,
                Value= true,
                QuestionKey = "newrawmaterials",
                QuestionText = "New raw materials?"
            },
               new()
            {
                Category = PlannerCategory.RawMaterials,
                Value= true,
                QuestionKey = "customersupplied",
                QuestionText = "Customer-supplied?"
            },
                   new()
            {
                Category = PlannerCategory.RawMaterials,
                Value= true,
                QuestionKey = "fromnewvendor",
                QuestionText = "From new vendor?"
            }

        };
        npiRecord.PilotRequirements = new List<PilotRequirement>
        {
            new()
            {
                QuestionKey = "benchtoppilotrequired",
                QuestionText = "Bench-Top Pilot Required"
            },
            new()
            {
                QuestionKey = "nongmpproductionpilotrequired",
                QuestionText = "Non-GMP Production Pilot Required"
            },
            new()
            {
                QuestionKey = "gmpproductionpilotrequired",
                QuestionText = "GMP Production Pilot Required"
            },
            new()
            {
                QuestionKey = "fillstudyrequired",
                QuestionText = "Fill Study Required"
            },
            new()
            {
                QuestionKey = "stabilityorapprovalsamplesrequired",
                QuestionText = "Stability or Approval Samples Required"
            }
        };
        await _uow.NpiRecords.AddAsync(npiRecord);
        await _uow.SaveChangesAsync();

        return Ok("Data inserted");
    }


    [HttpDelete("remove-duplicates/{itemCode}")]
    public async Task<IActionResult> RemoveDuplicates(string itemCode)
    {
        var removedCount = await _uow.NpiRecords
            .RemoveDuplicatesByItemCodeAsync(itemCode);

        if (removedCount == 0)
            return Ok("No duplicates found.");

        await _uow.SaveChangesAsync();

        return Ok($"{removedCount} duplicate record(s) removed.");
    }



}

// ══════════════════════════════════════════════════════
// DTOs & Request Models
// ══════════════════════════════════════════════════════



public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

public class ToggleRequest
{
    public bool Value { get; set; }
    public string? AnsweredBy { get; set; }
}
