using System;

namespace X01
{
    public abstract class ADisposable : IDisposable
    {
        protected virtual void OnDisposing(){
        }
        public void Dispose()
        {
  