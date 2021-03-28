﻿using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using FluentResults;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public interface IImageFetcher
    {
        Task<Result<Stream?>> Get(string url);
    }
}