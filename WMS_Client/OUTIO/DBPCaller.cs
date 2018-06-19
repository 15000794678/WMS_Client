using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phicomm_WMS.DB;
using System.Data;
using System.Diagnostics;

namespace Phicomm_WMS.OUTIO
{
    class DBPCaller
    {
        public static void InitReplenishProcess(string user, int stationId)
        {
            try
            {
                InitReplenishProcess ir = new InitReplenishProcess(user, stationId);
                ir.ExecuteQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static void InitPickProcess(string user, int stationId)
        {
            try
            {
                InitPickProcess ip = new InitPickProcess(user, stationId);
                ip.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InitPartialPickStation(string user, int stationId)
        {
            try
            {
                InitPartialPickStation ip = new InitPartialPickStation(user, stationId);
                ip.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeinitProcess(int stationId)
        {
            try
            {
                DeinitProcess dp = new DeinitProcess(stationId);
                dp.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool KeepAlive(int stationId)
        {
            try
            {
                Trace.WriteLine("Debug: --------KeepAlive Start---------");
                KeepAlive dk = new KeepAlive(stationId);
                dk.ExecuteQuery();

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------KeepAlive Stop---------");
            }
        }

        public static int CheckStationWorkState(int stationId)
        {
            try
            {
                Trace.WriteLine("Debug: --------CheckStationWorkState Start---------");
                WorkingStateChecker ws = new WorkingStateChecker(stationId);
                ws.ExecuteQuery();

                return ws.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------CheckStationWorkState Stop---------");
            }
        }

        public static string CreatePickTask2(string woid, int stockNoType, int type)
        {
            try
            {
                Trace.WriteLine("Debug:------CreatePickTask2 Start-------");
                CreatePickTask2 cp = new CreatePickTask2(woid, stockNoType, type);
                cp.ExecuteQuery();

                return cp.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug:------CreatePickTask2 Stop-------");
            }
        }

        public static string CreatePickTask(string woid, int stockNoType)
        {
            try
            {
                Trace.WriteLine("Debug:------CreatePickTask Start-------");       
                CreatePickTask cp = new CreatePickTask(woid, stockNoType);
                cp.ExecuteQuery();
                
                return cp.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug:------CreatePickTask Stop-------");
            }
        }

        public static string CreatePartialPickTask(string woid, int stockNoType)
        {
            try
            {
                Trace.WriteLine("Debug:------CreatePartialPickTask Start-------");
                CreatePartialPickTask cp = new CreatePartialPickTask(woid, stockNoType);
                cp.ExecuteQuery();

                return cp.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug:------CreatePartialPickTask Stop-------");
            }
        }

        public static DataTable AtStationPod(int stationId)
        {
            try
            {
                Trace.WriteLine("Debug: --------AtStationPod Start---------");
                AtStationPod ap = new AtStationPod(stationId);
                ap.ExecuteQuery();

                return ap.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------AtStationPod Stop---------");
            }
        }

        public static DataTable PickAtStationLoc(int stationId)
        {
            try
            {
                Trace.WriteLine("Debug: --------PickAtStationLoc Start---------");
                PickAtStationLoc ps = new PickAtStationLoc(stationId);
                ps.ExecuteQuery();

                return ps.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------PickAtStationLoc Stop---------");
            }
        }

        public static DataTable ReplenishAtStationLoc(int stationId)
        {
            try
            {
                Trace.WriteLine("Debug: --------ReplenishAtStationLoc Start---------");

                ReplenishAtStationLoc ps = new ReplenishAtStationLoc(stationId);
                ps.ExecuteQuery();

                return ps.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------ReplenishAtStationLoc Stop---------");
            }
        }

        /// <summary>
        /// 检测条码类型 1：储位编号 2：周转箱编号 3：TR_SN 4：出库单 5：入库单
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public static int CheckBarcode(string barcode)
        {
            try
            {
                Trace.WriteLine("Debug: --------CheckBarcode Start---------");
                BarcodeChecker checker = new BarcodeChecker(barcode);

                checker.ExecuteQuery();

                return checker.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------CheckBarcode Stop---------");
            }
        }

        public static int CompletePickMaterialByTrSn(int stationId, string stockNo, string locId, string trSn, int stockNoType)
        {
            try
            {
                Trace.WriteLine("Debug: --------CompletePickMaterialByTrSn Start---------");
                Trace.WriteLine("Debug: stationId=" + stationId + ", stockNo=" + stockNo + ", locId=" + locId + ", trSn=" + trSn + ", stockNoType=" + stockNoType);

                CompletePickMaterialByTrSn complete = new CompletePickMaterialByTrSn(stationId,
                                                        stockNo,                                                        
                                                        trSn,
                                                        locId,
                                                        stockNoType);
                complete.ExecuteQuery();

                return complete.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------CompletePickMaterialByTrSn Stop---------");
            }
        }

        public static int CompletePartialPickByTrSn(int stationId, string stockNo, string locId, string trSn, int stockNoType, ref int num)
        {
            try
            {
                Trace.WriteLine("Debug: --------CompletePartialPicklByTrSn Start---------");
                Trace.WriteLine("Debug: stationId=" + stationId + ", stockNo=" + stockNo + ", locId=" + locId + ", trSn=" + trSn + ", stockNoType=" + stockNoType);

                CompletePartialPicklByTrSn complete = new CompletePartialPicklByTrSn(stationId,
                                                        stockNo,
                                                        trSn,
                                                        locId,
                                                        stockNoType);
                complete.ExecuteQuery();

                num = complete.GetNum();

                return complete.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------CompletePartialPicklByTrSn Stop---------");
            }
        }

        public static int CompletePickMaterialByTrSn2(int stationId, string stockNo, string locId, string trSn, int stockNoType)
        {
            try
            {
                Trace.WriteLine("Debug: --------CompletePickMaterialByTrSn2 Start---------");
                Trace.WriteLine("Debug: stationId=" + stationId + ", stockNo=" + stockNo + ", locId=" + locId + ", trSn=" + trSn + ", stockNoType=" + stockNoType);

                CompletePickMaterialByTrSn2 complete = new CompletePickMaterialByTrSn2(stationId,
                                                        stockNo,
                                                        trSn,
                                                        locId,
                                                        stockNoType,
                                                        1);
                complete.ExecuteQuery();

                return complete.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------CompletePickMaterialByTrSn2 Stop---------");
            }
        }

        public static int CompletePickMaterialByLocId(int stationId, string stockNo, string locId, int stockNoType)
        {
            try
            {
                Trace.WriteLine("Debug: --------CompletePickMaterialByLocId Start---------");
                Trace.WriteLine("Debug: stationId=" + stationId + ", stockNo=" + stockNo + ", locId=" + locId + ", stockNoType=" + stockNoType);

                CompletePickMaterialByLocId complete = new CompletePickMaterialByLocId(stationId,
                                                        stockNo,
                                                        locId,
                                                        stockNoType);
                complete.ExecuteQuery();

                return complete.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------CompletePickMaterialByLocId Stop---------");
            }
        }

        public static int CompletePartialPickByLocId(int stationId, string stockNo, string locId, int stockNoType)
        {
            try
            {
                Trace.WriteLine("Debug: --------CompletePartialPickByLocId Start---------");
                Trace.WriteLine("Debug: stationId=" + stationId + ", stockNo=" + stockNo + ", locId=" + locId + ", stockNoType=" + stockNoType);

                CompletePartialPickByLocId complete = new CompletePartialPickByLocId(stationId,
                                                        stockNo,
                                                        locId,
                                                        stockNoType);
                complete.ExecuteQuery();

                return complete.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------CompletePartialPickByLocId Stop---------");
            }
        }

        public static int CompleteReelSplit(int stationId, string stockNo, int stockNoType, string newTrSn, string oldTrSn, int sendQty)
        {
            try
            {
                CompleteReelSplit rs = new CompleteReelSplit(stationId, stockNo, stockNoType, newTrSn, oldTrSn, sendQty);
                rs.ExecuteQuery();

                return rs.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public static int AgvReturn(int stationId)
        {
            try
            {
                Trace.WriteLine("Debug: --------AgvReturn Start---------");

                BotContinueWorking continueWorking = new BotContinueWorking(stationId);
                continueWorking.ExecuteQuery();
                return continueWorking.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug: --------AgvReturn Stop---------");
            }
        }

        public static int CompleteReplenishMaterial(int stationId, string locId, int holderId, string stockno)
        {
            try
            {
                Trace.WriteLine("Debug:-----------CompleteReplenishMaterial Start---------------");
                Trace.WriteLine("Debug: stationId=" + stationId + ", locId=" + locId + ", holderId=" + holderId + ", stockno=" + stockno);

                CompleteReplenishMaterial sr = new CompleteReplenishMaterial(stationId, locId, holderId, stockno);
                sr.ExecuteQuery();

                return sr.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug:-----------CompleteReplenishMaterial Stop---------------");
            }
        }

        public static int CompleteReplenishMaterial2(int stationId, string locId, int holderId, string stockno, int type)
        {
            try
            {
                Trace.WriteLine("Debug:-----------CompleteReplenishMaterial2 Start---------------");
                Trace.WriteLine("Debug: stationId=" + stationId + ", locId=" + locId + ", holderId=" + holderId + ", stockno=" + stockno + ", type=" + type);

                CompleteReplenishMaterial2 sr = new CompleteReplenishMaterial2(stationId, locId, holderId, stockno, type);
                sr.ExecuteQuery();

                return sr.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug:-----------CompleteReplenishMaterial2 Stop---------------");
            }
        }

        public static int LinkHolderMaterial(string trSn, string stockno, string materialId, string dateCode, string fifoDateCode,
            int qty, int holderId, int stationId, string venderId, string woidtype, int stocknotype)
        {
            try
            {
                Trace.WriteLine("Debug:-----------LinkHolderMaterial Start-------------");
                //打印输入参数
                Trace.WriteLine("Debug: trsn=" + trSn + ", stockno=" + stockno + ", materialId=" + materialId + ", dateCode=" + dateCode +
                                ", fifoDateCode=" + fifoDateCode + ", qty=" + qty.ToString() + ", holderId=" + holderId + 
                                ", stationId=" + stationId + ", venderId=" + venderId + ", woidtype=" + woidtype + ", stocknotype=" + stocknotype);

                LinkHolderMaterial lm = new LinkHolderMaterial(trSn, stockno, materialId, dateCode, fifoDateCode, qty, holderId, stationId, venderId, woidtype, stocknotype);
                lm.ExecuteQuery();

                return lm.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug:-----------LinkHolderMaterial Stop-------------");
            }
        }

        public static string CheckMaterialIntoHolders(int stationId, string stockno)
        {
            try
            {
                Trace.WriteLine("Debug:-----------CheckMaterialIntoHolders Start-------------");
                CheckMaterialIntoHolders cm = new DB.CheckMaterialIntoHolders(stationId, stockno);
                cm.ExecuteQuery();

                return cm.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug:-----------CheckMaterialIntoHolders Stop-------------");
            }
        }

        public static string CheckMaterialIntoHolders2(int stationId, string stockno, int type)
        {
            try
            {
                Trace.WriteLine("Debug:-----------CheckMaterialIntoHolders2 Start-------------");
                CheckMaterialIntoHolders2 cm = new DB.CheckMaterialIntoHolders2(stationId, stockno, type);
                cm.ExecuteQuery();

                return cm.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug:-----------CheckMaterialIntoHolders2 Stop-------------");
            }
        }

        public static int ReplenishToPickDirectly(int stationId, string stockInNo, int stockInType, string subType, string stockOutNo, int stockOutType,
                string trSn, string materialNo, string dateCode, string fifoDc, int qty, string vendorNo)
        {
            try
            {
                Trace.WriteLine("Debug:-----------ReplenishToPickDirectly Start-------------");

                ReplenishToPickDirectly rt = new ReplenishToPickDirectly(stationId, stockInNo, stockInType, subType, stockOutNo, stockOutType, 
                                                                         trSn, materialNo, dateCode, fifoDc, qty, vendorNo);
                rt.ExecuteQuery();

                return rt.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Trace.WriteLine("Debug:-------------ReplenishToPickDirectly Stop-------------");
            }
        }
    }
}
