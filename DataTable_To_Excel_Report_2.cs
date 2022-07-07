        [HttpGet]
        [Route("api/PayTypeWiseSaleReport/{type}/{company_code}/{store_code}/{sDate}/{eDate}")]
        public IHttpActionResult PayTypeWiseSaleReport(string type, string company_code, string store_code, DateTime sDate, DateTime eDate)
        {
            try
            {
                var request = Request;
                if (request.Headers.Contains("Authorization"))
                {
                    var auth = request.Headers.GetValues("Authorization").ToList();
                    var headers = auth[0].Split(':');
                    string username = headers[0];
                    string password = headers[1];
                    decimal total = 0;
                    DataTable items = service.PayTypeWiseSaleReport(sDate, eDate, company_code, store_code, username, password);
                    items.TableName = "MyTableName";


                    items.Columns.Add("Total", typeof(System.Decimal));
                    foreach (DataRow item in items.Rows)
                    {                    

                        for (int i = 7; i < item.Table.Columns.Count - 1; i++)
                        {
                            total += Convert.ToDecimal(item[i].ToString());
                        }

                        item["Total"] = total.ToString();
                        total = 0;
                    }

                    items.Columns["INVOICE_DT"].ColumnName = "Date";
                    items.Columns["ITEM_PRICE"].ColumnName = "Sales";
                    items.Columns["NET_AMT"].ColumnName = "Net Sales With VAT";
                    items.Columns["DISC_AMT"].ColumnName = "Discount";
                    items.Columns["VAT_AMT"].ColumnName = "VAT";
                    items.Columns["ADJ_AMT"].ColumnName = "Adj Amt";
                    items.Columns["PAYMENT_CODE"].ColumnName = "PAYMENT CODE";

                    if (items.Rows.Count == 0)
                    {
                        Log.Debug("Not Found");
                        return Content(HttpStatusCode.NotFound, "Sorry!!! Data Not Found");
                    }
                    else
                    {

                        Guid guid = Guid.NewGuid();
                        string filepath = System.Web.HttpContext.Current.Server.MapPath("~/CReports") + "/" + guid.ToString() + "." + "xls";                        
                        items.WriteXml(filepath);                      

                        string filename = "/CReports/" + guid.ToString() + ".xls";
                        return Content(HttpStatusCode.OK, filename);
                    }
                }
                else
                {
                    Log.Debug("Bad Sale Request");
                    return Content(HttpStatusCode.BadRequest, "Bad Sale Request");
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
