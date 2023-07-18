using System;
using Xunit;
namespace dotnet.Test.Framework;

public abstract class SpecFor 
{
    public void SetUp()
    {
        Context();
        Because();
    }

    protected abstract void Because();
    protected abstract void Context();

    protected abstract void CleanUp();
}

public abstract class SpecFor<T> : IDisposable
{
    public SpecFor()
    {
        this.SetUp();
    }
    protected T subject;
    public virtual void SetUp()
    {
        Context();
        Because();
    }

    public abstract void Because();
    public abstract void Context();

    protected virtual void CleanUp()
    {

    }

    public void Dispose()
    {
        CleanUp();
    }
}