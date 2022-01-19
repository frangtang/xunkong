﻿using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Xunkong.Core.Hoyolab;

namespace Xunkong.Core.Wish
{
    public class WishlogClient
    {
        private static readonly string cnUrl = "https://hk4e-api.mihoyo.com/event/gacha_info/api/getGachaLog";
        private static readonly string seaUrl = "https://hk4e-api-os.mihoyo.com/event/gacha_info/api/getGachaLog";

        private readonly string? baseRequestUrl;

        /// <summary>
        /// 祈愿记录网址的查询字符串，包含开头的「?」
        /// </summary>
        private readonly string? authString;

        private static HttpClient HttpClient;

        public event EventHandler<(WishType WishType, int Page)>? ProgressChanged;


        static WishlogClient()
        {
            HttpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.All });
            HttpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">祈愿记录网页，以 https://webstatic.mihoyo.com 或 https://webstatic-sea.mihoyo.com 开头，以 #/log 结尾</param>
        /// <exception cref="ArgumentException">输入的Url不符合要求</exception>
        public WishlogClient(string url)
        {
            var match = Regex.Match(url, @"(https://webstatic.+#/log)");
            if (!match.Success)
            {
                throw new ArgumentException("Url does not meet the requirement.");
            }
            url = match.Groups[1].Value;
            authString = url.Substring(url.IndexOf('?')).Replace("#/log", "");
            if (url.Contains("webstatic-sea"))
            {
                baseRequestUrl = seaUrl;
            }
            else
            {
                baseRequestUrl = cnUrl;
            }
        }




        /// <summary>
        /// 获取一页祈愿数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns>没有数据时返回空集合</returns>
        /// <exception cref="HoyolabException">api请求返回值不为零时抛出异常</exception>
        private async Task<List<WishlogItem>> GetWishlogByParamAsync(QueryParam param, CancellationToken cancellationToken = default)
        {
            var url = $"{baseRequestUrl}{authString}&{param}";
            var response = await HttpClient.GetFromJsonAsync<WishlogResponseData>(url, cancellationToken);
            if (response is null)
            {
                throw new HoyolabException(-1, "Cannot parse the return data.");
            }
            if (response.Retcode != 0)
            {
                throw new HoyolabException(response.Retcode, response.Message ?? "No return meesage.");
            }
            return response.Data?.List ?? new(0);
        }


        /// <summary>
        /// 获取一种卡池类型的祈愿数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lastId">获取的祈愿id小于最新id即停止</param>
        /// <param name="size">每次api请求获取几条数据，不超过20，默认6</param>
        /// <returns>没有数据返回空集合</returns>
        /// <exception cref="HoyolabException">api请求返回值不为零时抛出异常</exception>
        private async Task<List<WishlogItem>> GetWishlogByTypeAsync(WishType type, long lastId = 0, int size = 6)
        {
            var param = new QueryParam(type, 1, size);
            var result = new List<WishlogItem>();
            while (true)
            {
                ProgressChanged?.Invoke(this, (type, param.Page));
                var list = await GetWishlogByParamAsync(param);
                result.AddRange(list);
                if (list.Count == size && list.Last().Id < lastId)
                {
                    param.Page++;
                    param.EndId = list.Last().Id;
                    await Task.Delay(Random.Shared.Next(200, 300));
                }
                else
                {
                    break;
                }
            }
            foreach (var item in result)
            {
                item.QueryType = type;
            }
            return result;
        }



        /// <summary>
        /// 获取所有的祈愿数据，以id顺序排列
        /// </summary>
        /// <param name="lastId">获取的祈愿id小于最新id即停止</param>
        /// <param name="size">每次api请求获取几条数据，不超过20，默认6</param>
        /// <returns>没有数据返回空集合</returns>
        /// <exception cref="HoyolabException">api请求返回值不为零时抛出异常</exception>
        public async Task<List<WishlogItem>> GetAllWishlogAsync(int lastId = 0, int size = 6)
        {
            lastId = lastId < 0 ? 0 : lastId;
            size = Math.Clamp(size, 1, 20);
            var result = new List<WishlogItem>();
            result.AddRange(await GetWishlogByTypeAsync(WishType.Novice, lastId, size));
            result.AddRange(await GetWishlogByTypeAsync(WishType.Permanent, lastId, size));
            result.AddRange(await GetWishlogByTypeAsync(WishType.CharacterEvent, lastId, size));
            result.AddRange(await GetWishlogByTypeAsync(WishType.WeaponEvent, lastId, size));
            return result.OrderBy(x => x.Id).ToList();
        }



        /// <summary>
        /// 获取祈愿记录网址所属的Uid
        /// </summary>
        /// <returns>返回值为0代表没有祈愿数据</returns>
        /// <exception cref="HoyolabException">api请求返回值不为零时抛出异常</exception>
        /// <exception cref="ArgumentNullException">祈愿记录网址为空</exception>
        public async Task<int> GetUidAsync()
        {
            if (string.IsNullOrWhiteSpace(authString))
            {
                throw new ArgumentNullException(nameof(authString));
            }
            var param = new QueryParam(WishType.CharacterEvent, 1);
            var list = await GetWishlogByParamAsync(param);
            if (list.Any())
            {
                return list.First().Uid;
            }
            await Task.Delay(Random.Shared.Next(200, 300));
            param.WishType = WishType.Permanent;
            list = await GetWishlogByParamAsync(param);
            if (list.Any())
            {
                return list.First().Uid;
            }
            await Task.Delay(Random.Shared.Next(200, 300));
            param.WishType = WishType.WeaponEvent;
            list = await GetWishlogByParamAsync(param);
            if (list.Any())
            {
                return list.First().Uid;
            }
            await Task.Delay(Random.Shared.Next(200, 300));
            param.WishType = WishType.Novice;
            list = await GetWishlogByParamAsync(param);
            if (list.Any())
            {
                return list.First().Uid;
            }
            return 0;
        }




    }
}
