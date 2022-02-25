namespace Utils
{
    public static class AsyncExtensionMethods
    {
        /// <summary>
        /// https://gist.github.com/mattyellen/d63f1f557d08f7254345bff77bfdc8b3
        /// Allows the use of async/await (instead of yield) with any Unity AsyncOperation.
        /// Added because of editor bug that prevented using UnityWebRequest as async even though Unity already supports async/await.
        /// </summary>
        /// <param name="asyncOp"></param>
        /// <returns></returns>
        //public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        //{
        //    var tcs = new TaskCompletionSource<object>();
        //    asyncOp.completed += obj => { tcs.SetResult(null); };
        //    return ((Task)tcs.Task).GetAwaiter();
        //}
    }
}
