using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using DragonService.Model;
using Dragons.Shared;
using Newtonsoft.Json;
using System.IO;


namespace Dragons.Service
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            HttpContext.Current.Cache.Insert(Constants.LobbyKey, new Dictionary<string, LobbyClient>());
            HttpContext.Current.Cache.Insert(Constants.MatchesKey, new Dictionary<string, Match>());
            HttpContext.Current.Cache.Insert(Constants.MatchMovesKey, new Dictionary<string, List<Move>>());

            // Cache settings
            HttpContext.Current.Cache.Insert(Constants.InitialManaKey, Constants.DefaultInitialMana);
            HttpContext.Current.Cache.Insert(Constants.ManaIncrementKey, Constants.DefaultManaIncrement);
            HttpContext.Current.Cache.Insert(Constants.GridSizeKey, Constants.DefaultGridSize);

            // Cache layouts
           LoadLayouts();
        }

        private void LoadLayouts()
        {
            List<Grid> Layouts = new List<Grid>();
            List<Grid> PieceLists = new List<Grid>();

            foreach (string layoutFilePath in Directory.EnumerateFiles(Server.MapPath("~/App_Data/Layouts"), "*.json"))
            {
                using (StreamReader layoutStream = File.OpenText(layoutFilePath))
                {
                    Grid pieceList = JsonConvert.DeserializeObject<Grid>(layoutStream.ReadToEnd());
                    int gridSize = (int)HttpContext.Current.Cache[Constants.GridSizeKey];
                    Grid layoutGrid = new Grid(gridSize);

                    // Grid : List<List<Cell>>

                    foreach (List<Cell> piece in pieceList)
                    {
                        foreach (Cell cell in piece)
                        {
                            cell.LastMoves = new Stack<string>();
                            cell.Coordinate.x -= 1; // files are not zero-based.  remove -1 is changed to zero based.
                            cell.Coordinate.y -= 1; // files are not zero-based.  remove -1 is changed to zero based.
                            layoutGrid[cell.Coordinate.x][cell.Coordinate.y] = cell;
                        }
                    }

                    /*
                    using (StreamWriter sw = File.CreateText(Server.MapPath("~/App_Data/layout_out.json")))
                    {
                        sw.Write(JsonConvert.SerializeObject(layoutGrid));
                    }*/

                    Layouts.Add(layoutGrid);
                    PieceLists.Add(pieceList);
                }
            }

            HttpContext.Current.Cache.Insert(Constants.LayoutsKey, Layouts);
            HttpContext.Current.Cache.Insert(Constants.PieceListsKey, PieceLists);
        }
    }
}