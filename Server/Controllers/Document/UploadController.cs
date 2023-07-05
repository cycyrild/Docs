using DocsWASM.Client.Pages;
using DocsWASM.Server;
using System.IO;
using DocsWASM.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Reflection.Metadata;
using System.Security.Claims;
using static DocsWASM.Shared.AccountModels;
using static DocsWASM.Shared.UploadModels;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net;
using SkiaSharp;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static DocsWASM.Shared.ImageProcessing;
using static DocsWASM.Server.Helpers.SvgProcessing;
using System.Configuration;
using DocsWASM.Shared.Serializer;

namespace DocsWASM.Server.Controllers.Document
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {

        private readonly ILogger<UploadController> _logger;
        public AppDb Db { get; }
        public UploadController(ILogger<UploadController> logger, AppDb db)
        {
            _logger = logger;
            Db = db;
        }

        [HttpPost("uploadDocuments")]
        public async Task<UploadStatus> Upload()
        {
            using (var ms = new MemoryStream(1024 * 20000))
            {
                await Request.Body.CopyToAsync(ms);
                ms.Position = 0;
                var byteArray = ms.ToArray();

				var uploadSend = UploadSendModelSerializer.Deserialize(byteArray);
                var userId = uint.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                try
				{
                    var form = uploadSend.Upload;
                    var pageModels = uploadSend.Pages;
                    await Db.Connection.OpenAsync();
                    MySqlCommand cmd;
                    ulong? docId = null;

                    cmd = Db.Connection.CreateCommand();
                    cmd.CommandText = @"
					    insert into documents(
						    name,
						    description,
						    subjectId,
						    ownerUserId,
						    imgPreview,
						    docType,
						    yearGroup,
						    school,
						    chapterId,
                            approved
					    ) values (
						    @name,
						    @description,
						    @subjectId,
						    @ownerUserId,
						    @imgPreview,
						    @docType,
						    @yearGroup,
						    @school,
						    @chapterId,
                            @approved
					    );
					    SELECT LAST_INSERT_ID();";
                    cmd.Parameters.AddWithValue("@name", form.Name);
                    cmd.Parameters.AddWithValue("@description", form.Description);
                    cmd.Parameters.AddWithValue("@subjectId", form.SubjectId);
                    cmd.Parameters.AddWithValue("@ownerUserId", userId);
                    cmd.Parameters.AddWithValue("@imgPreview", pageModels[0].fileType != dataBinTypesEnum.svg ? ImgToWebP(pageModels[0].bin, 500, 100) : SvgToWebP(pageModels[0].bin, 500, 100));
                    cmd.Parameters.AddWithValue("@docType", form.DocumentTypeId);
                    cmd.Parameters.AddWithValue("@yearGroup", form.YearGroupName);
                    cmd.Parameters.AddWithValue("@school", form.SchoolName);
                    cmd.Parameters.AddWithValue("@chapterId", form.ChapterId);
                    cmd.Parameters.AddWithValue("@approved", 0);
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                            docId = (ulong)reader.GetValue(0);



                    cmd = Db.Connection.CreateCommand();

                    cmd.CommandText = @"
					    insert into pages(
						    pageNo,
						    documentId,
						    paragraphs,
						    name,
						    yearGroup,
						    school,
						    chapterId,
						    docType,
						    docBinType,
						    subjectId,
						    isCorrection,
						    bin,
                            placeHolder
						    ) values ";

                    for (int i = 0; i < pageModels.Count(); i++)
                    {
                        string cleanSvg = "";
                        if(pageModels[i].fileType == dataBinTypesEnum.svg)
                            cleanSvg = XMLHelper.XMLHelper.Clean(Encoding.UTF8.GetString(pageModels[i].bin));

                        cmd.CommandText += $"(@pageNo{i}, @documentId{i}, @paragraphs{i}, @name{i}, @yearGroup{i}, @school{i}, @chapterId{i}, @docType{i}, @docBinType{i}, @subjectId{i}, @isCorrection{i}, @bin{i}, @placeHolder{i}){(i == pageModels.Count() - 1 ? ";" : ",")}\n";
                        cmd.Parameters.AddWithValue($"@pageNo{i}", i + 1);
                        cmd.Parameters.AddWithValue($"@documentId{i}", docId.Value);
                        cmd.Parameters.AddWithValue($"@paragraphs{i}", pageModels[i].paragraphsString);
                        cmd.Parameters.AddWithValue($"@name{i}", form.Name);
                        cmd.Parameters.AddWithValue($"@yearGroup{i}", form.YearGroupName);
                        cmd.Parameters.AddWithValue($"@school{i}", form.SchoolName);
                        cmd.Parameters.AddWithValue($"@chapterId{i}", form.ChapterId);
                        cmd.Parameters.AddWithValue($"@docType{i}", form.DocumentTypeId);
                        cmd.Parameters.AddWithValue($"@docBinType{i}", (byte)pageModels[i].fileType);
                        cmd.Parameters.AddWithValue($"@subjectId{i}", form.SubjectId);
                        cmd.Parameters.AddWithValue($"@isCorrection{i}", pageModels[i].isCorrection);
                        cmd.Parameters.AddWithValue($"@bin{i}", pageModels[i].fileType == dataBinTypesEnum.svg ? Encoding.UTF8.GetBytes(cleanSvg) : pageModels[i].bin);
					    cmd.Parameters.AddWithValue($"@placeHolder{i}", pageModels[i].fileType != dataBinTypesEnum.svg ? ImgToWebP(pageModels[i].bin, 75, 75) : SvgToWebP(pageModels[i].bin, 75, 75));
				    }
				    await cmd.ExecuteNonQueryAsync();
                    return new UploadStatus() { documenId = (uint)docId, success = true };
                }
				catch (Exception ex)
				{
					return new UploadStatus() { errorMessage = ex.Message, success = false };
				}
            }
        }


    }
}
