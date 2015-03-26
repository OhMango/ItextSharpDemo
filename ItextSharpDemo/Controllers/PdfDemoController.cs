using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp;
using it = iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing;
using ItextSharpDemo.Models;
namespace ItextSharpDemo.Controllers
{
    public class PdfDemoController : Controller
    {
        //
        // GET: /PdfDemo/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetPdf()
        {
            it.Font font = new it.Font(BaseFont.CreateFont("C:\\Windows\\Fonts\\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 10);


            MemoryStream ms = new MemoryStream();
            it.Document document = new it.Document();

            PdfWriter.GetInstance(document, ms);

            document.Open();

            document.Add(new it.Paragraph("Yes Master!"));
            document.Add(new it.Paragraph("其疾如风,其徐如林,侵掠如火,不动如山,难知如阴,动如雷震", font));

            document.Close();
            return File(ms.ToArray(), "application/pdf", "ceshi.pdf");
        }
        public ActionResult ImagePdf()
        {
            return View();
        }
        public ActionResult SendImagePdfUrl(string ImageUrl)
        {
            JsonResult j = new JsonResult();
            string fileName = System.Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(ImageUrl))
            {
                Image pdfImage = base64ToPic(ImageUrl);
                pdfImage.Save(Server.MapPath("~") + "/pdfimage/" + fileName + "1.jpg");
                var data = new { message = "success", filename = fileName };
                j.Data = data;//返回单个对象；  
            }
            else
            {
                var data = new { message = "未提供Url" };
                j.Data = data;//返回单个对象； 
            }
            return j;
        }
        public ActionResult GetImagePdf(string ID)
        {
            it.Font font = new it.Font(BaseFont.CreateFont("C:\\Windows\\Fonts\\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 10);
            MemoryStream ms = new MemoryStream();
            it.Document document = new it.Document();
            PdfWriter.GetInstance(document, ms);
            document.Open();
            document.Add(new it.Paragraph("Yes Master!"));
            document.Add(new it.Paragraph("其疾如风,其徐如林,侵掠如火,不动如山,难知如阴,动如雷震", font));
            List<string> imageStringList = GetImageString(ID, 1);

            foreach (var item in imageStringList)
            {
                try
                {
                    //如果传过来的是Base64
                    //it.Image image = it.Image.GetInstance(base64ToPic(item), System.Drawing.Imaging.ImageFormat.Jpeg);
                    //如果传过来的是地址
                    it.Image image = it.Image.GetInstance(Server.MapPath("~") + "/pdfimage/" + item + ".jpg");

                    image.Alignment = it.Image.ALIGN_LEFT;
                    image.ScalePercent(30);
                    document.Add(image);
                }
                catch (Exception e)
                {
                    document.Add(new it.Paragraph("图片" + item + "不存在"));
                }
            }
            document.Close();
            document.Dispose();
            return File(ms.ToArray(), "application/pdf", "ceshi.pdf");
        }
        public ActionResult TablePdf()
        {
            return View();
        }
        public ActionResult GetTablePdf()
        {
            it.Font font = new it.Font(BaseFont.CreateFont("C:\\Windows\\Fonts\\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 10);
            MemoryStream ms = new MemoryStream();
            it.Document document = new it.Document();
            PdfWriter.GetInstance(document, ms);
            document.Open();
            document.Add(new it.Paragraph("Yes Master!"));
            document.Add(new it.Paragraph("其疾如风,其徐如林,侵掠如火,不动如山,难知如阴,动如雷震", font));
            List<Person> personList = new List<Person>();
            personList.Add(new Person {Name="wlz",Address="1111"});
            personList.Add(new Person { Name = "xiaoming", Address = "1112" });
            personList.Add(new Person { Name = "xiaohong", Address = "1113" });
            List<NameToColName> nameList = new List<NameToColName>();
            nameList.Add(new NameToColName { ModelName = "Name", ColName = "姓名" });
            nameList.Add(new NameToColName { ModelName = "Address", ColName = "地址" });
            var properties = personList.First().GetType().GetProperties() as System.Reflection.PropertyInfo[];
            PdfPTable nameTable = new PdfPTable(properties.Length);
            //创建表头
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                string colName = (nameList.Where(p => p.ModelName == property.Name).ToList())[0].ColName;
                nameTable.AddCell(new it.Phrase(colName,font));//注意加上中文字体
            }
            
            personList.ForEach(item =>
            {                              
                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    nameTable.AddCell(new it.Phrase(property.GetValue(item, null).ToString(),font));
                }
            });
            document.Add(nameTable);
            
            document.Close();
            document.Dispose();
            return File(ms.ToArray(), "application/pdf", "TableDemo.pdf");
        }
        /// <summary>
        /// //对字节数组字符串进行Base64解码并生成图片
        /// </summary>
        /// <param name="ImageUrl"></param>
        /// <returns></returns>
        public Image base64ToPic(string ImageUrl)
        {

            if (ImageUrl == null) //图像数据为空
            {
                return null;
            }
            try
            {
                //将一开始的data:png等信息去掉，只剩base64字符串
                String[] url = ImageUrl.Split(',');
                String u = url[1];
                //Base64解码
                byte[] imageBytes = Convert.FromBase64String(u);
                Image image;
                //生成图片
                using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    // Convert byte[] to Image
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    image = Image.FromStream(ms, true);
                }
                return image;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        /// <summary>
        /// 得到这一系列Image的Url
        /// </summary>
        /// <param name="ID">相同部分</param>
        /// <param name="count">共有几张</param>
        /// <returns></returns>
        public List<string> GetImageString(string ID, int count)
        {
            List<string> ImageStringList = new List<string>();

            for (int i = 0; i < count; i++)
            {
                ImageStringList.Add(ID + count.ToString());
            }
            return ImageStringList;
        }

    }
}
