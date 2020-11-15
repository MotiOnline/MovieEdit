using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieEdit;
using MovieEdit.Effects;
using MovieEdit.TL;
using MovieEditServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieEditServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimelineController : ControllerBase
    {
        // PUT: api/TodoItems/5
        [HttpPut]
        public async Task<IActionResult> PutComponent(EditJson json)
        {
            OpenCvSharp.VideoCapture cap = new OpenCvSharp.VideoCapture(@"D:\Movie\piano.mp4");
            Dictionary<FrameInfo, PrintEffectBase> effect = new();
            //var tl = Project.OpeningProject.Timeline;
            foreach (var c in json.Components)
            {
                foreach (var b in c.Blocks)
                {
                    if (b.Kind == "DefineComponentBlock") continue;
                    var t = Type.GetType($"MovieEditServer.Models.{b.Kind}");
                    if (b is MovieLoadingBlock mlb)
                    {
                        cap = new OpenCvSharp.VideoCapture(mlb.MaterialPath);
                        //tl.AddObject(1, new FrameInfo((uint)json.Clips.Position.X, json.Clips.Width), new TimelineMovie(new PositionInfo(0, 0), mlb.MaterialPath));
                    }
                    if (b is GrayScaleFilterBlock obj)
                    {
                        effect.Add(new FrameInfo((uint)json.Clips.Position.X, json.Clips.Width), PrintEffect.COLORCONVERT(OpenCvSharp.ColorConversionCodes.GRAY2BGR));
                        //var tm = tl.GetObject(1, new FrameInfo((uint)json.Clips.Position.X, json.Clips.Width)) as TimelineMovie;
                        //tm.AddEffect(PrintEffect.COLORCONVERT(OpenCvSharp.ColorConversionCodes.GRAY2BGR));
                    }
                }
            }
            MovieEdit.IO.Movie.OutputMovie("./test", ".mp4", OpenCvSharp.FourCC.H264, cap, effect);
            return NoContent();
        }
    }
}
