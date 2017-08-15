using Extract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Extract
{
	public class ExtractController : ApiController
	{

		[HttpPost]
		[Route("api/upload")]
		public Task<DataFile> Upload(string database = null) {

			if (!string.IsNullOrWhiteSpace(database)) {
				database = ValidateDatabaseName(database);
			}

			HttpRequestMessage request = this.Request;
			if (!request.Content.IsMimeMultipartContent()) throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

			string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/uploads");
			MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(root);

			Task<DataFile> task = request.Content.ReadAsMultipartAsync(provider).ContinueWith<DataFile>(o => {

				MultipartFileData data = provider.FileData.First();
				string localFileName = data.LocalFileName;
				string fileName = data.Headers.ContentDisposition.FileName;

				DataFile file = new DataFile(localFileName, fileName, database);
				IDataLoader loader = DataLoaderFactory.CreateDataLoader(file.type, database);
				loader.Import(file);

				return file;
			});

			return task;
		}


		[HttpGet]
		[Route("api/data")]
		public DataModel GetAllData(string database) {
			IDataController controller = DataControllerFactory.CreateDataController(database);
			DataModel data = controller.GetData();
			return data;
		}


		[HttpGet]
		[Route("api/metadata")]
		public DataModel GetMetaData(string database) {
			IDataController controller = DataControllerFactory.CreateDataController(database);
			DataModel data = controller.GetMetaData();
			return data;
		}


		[HttpGet]
		[Route("api/tabledata")]
		public List<ColumnModel> GetTableData(string database, string table) {
			IDataController controller = DataControllerFactory.CreateDataController(database);
			List<ColumnModel> tabledata = controller.GetColumns(table);
			return tabledata;
		}


		[HttpGet]
		[Route("api/export")]
		public string GetExport(string database, string type) {
			IDataLoader loader = DataLoaderFactory.CreateDataLoader(DataTypeConverter.Convert(type), database);
			string relativePath = loader.Export(database);
			string downloadUrl = Path.Combine(Request.RequestUri.GetLeftPart(UriPartial.Authority), relativePath);
			return downloadUrl;
		}


		private string ValidateDatabaseName(string database) {
			string[] idArray = database.Split('-');
			if (idArray.Length != 5) return null;
			if (idArray[0].ToCharArray().Length != 8) return null;
			if (idArray[1].ToCharArray().Length != 4) return null;
			if (idArray[2].ToCharArray().Length != 4) return null;
			if (idArray[3].ToCharArray().Length != 4) return null;
			if (idArray[4].ToCharArray().Length != 12) return null;
			return database;
		}
	}
}
