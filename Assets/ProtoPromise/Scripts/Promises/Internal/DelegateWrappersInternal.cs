﻿#if PROTO_PROMISE_DEBUG_ENABLE || (!PROTO_PROMISE_DEBUG_DISABLE && DEBUG)
#define PROMISE_DEBUG
#else
#undef PROMISE_DEBUG
#endif

#pragma warning disable RECS0108 // Warns about static fields in generic types
#pragma warning disable IDE0018 // Inline variable declaration
#pragma warning disable IDE0034 // Simplify 'default' expression
#pragma warning disable RECS0001 // Class is declared partial but has only one part

using System;
using Proto.Utils;

namespace Proto.Promises
{
    partial class Promise
    {
        partial class InternalProtected
        {
#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateResolvePassthrough : IDelegateResolve, IDelegateResolvePromise
            {
                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    owner.ResolveInternal(valueContainer);
                }

                public void MaybeUnregisterCancelation() { }
                public bool IsNull { get { throw new System.InvalidOperationException(); } }
            }

            #region Regular Delegates
#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal sealed class FinallyDelegate : Internal.ITreeHandleable, Internal.ITraceable
            {
#if PROMISE_DEBUG
                Internal.CausalityTrace Internal.ITraceable.Trace { get; set; }
#endif
                Internal.ITreeHandleable ILinked<Internal.ITreeHandleable>.Next { get; set; }

                private static ValueLinkedStack<Internal.ITreeHandleable> _pool;

                private Action _onFinally;

                private FinallyDelegate() { }

                static FinallyDelegate()
                {
                    Internal.OnClearPool += () => _pool.Clear();
                }

                public static FinallyDelegate GetOrCreate(Action onFinally)
                {
                    var del = _pool.IsNotEmpty ? (FinallyDelegate) _pool.Pop() : new FinallyDelegate();
                    del._onFinally = onFinally;
                    SetCreatedStacktrace(del, 2);
                    return del;
                }

                private void InvokeAndCatchAndDispose()
                {
                    var callback = _onFinally;
                    SetCurrentInvoker(this);
                    Dispose();
                    try
                    {
                        callback.Invoke();
                    }
                    catch (Exception e)
                    {
                        Internal.AddRejectionToUnhandledStack(e, this);
                    }
                    ClearCurrentInvoker();
                }

                void Dispose()
                {
                    _onFinally = null;
                    if (Config.ObjectPooling != PoolType.None)
                    {
                        _pool.Push(this);
                    }
                }

                void Internal.ITreeHandleable.Handle()
                {
                    InvokeAndCatchAndDispose();
                }

                void Internal.ITreeHandleable.MakeReady(Promise owner, Internal.IValueContainer valueContainer, ref ValueLinkedQueue<Internal.ITreeHandleable> handleQueue)
                {
                    handleQueue.Push(this);
                }

                void Internal.ITreeHandleable.MakeReadyFromSettled(Promise owner, Internal.IValueContainer valueContainer)
                {
                    Internal.AddToHandleQueueBack(this);
                }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateVoidVoid : IDelegateResolve, IDelegateReject, IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly Action _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateVoidVoid(Action callback)
                {
                    _callback = callback;
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke();
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke();
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateArgVoid<TArg> : IDelegateResolve, IDelegateReject, IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly Action<TArg> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateArgVoid(Action<TArg> callback)
                {
                    _callback = callback;
                }

                private void Invoke(TArg arg, Promise owner)
                {
                    _callback.Invoke(arg);
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value, owner);
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg;
                    if (Internal.TryConvert(valueContainer, out arg))
                    {
                        Invoke(arg, owner);
                    }
                    else
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateVoidResult<TResult> : IDelegateResolve, IDelegateReject, IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly Func<TResult> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateVoidResult(Func<TResult> callback)
                {
                    _callback = callback;
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke();
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke();
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateArgResult<TArg, TResult> : IDelegateResolve, IDelegateReject, IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly Func<TArg, TResult> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateArgResult(Func<TArg, TResult> callback)
                {
                    _callback = callback;
                }

                private void Invoke(TArg arg, Promise owner)
                {
                    var temp = _callback;
                    TResult result = temp.Invoke(arg);
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value, owner);
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg;
                    if (Internal.TryConvert(valueContainer, out arg))
                    {
                        Invoke(arg, owner);
                    }
                    else
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                }

                public void MaybeUnregisterCancelation() { }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateVoidPromise : IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly Func<Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateVoidPromise(Func<Promise> callback)
                {
                    _callback = callback;
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke());
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke());
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateArgPromise<TArg> : IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly Func<TArg, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateArgPromise(Func<TArg, Promise> callback)
                {
                    _callback = callback;
                }

                private void Invoke(TArg arg, Promise owner)
                {
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke(arg));
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value, owner);
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg;
                    if (Internal.TryConvert(valueContainer, out arg))
                    {
                        Invoke(arg, owner);
                    }
                    else
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateVoidPromiseT<TPromise> : IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly Func<Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateVoidPromiseT(Func<Promise<TPromise>> callback)
                {
                    _callback = callback;
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke());
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke());
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateArgPromiseT<TArg, TPromise> : IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly Func<TArg, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateArgPromiseT(Func<TArg, Promise<TPromise>> callback)
                {
                    _callback = callback;
                }

                private void Invoke(TArg arg, Promise owner)
                {
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke(arg));
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value, owner);
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg;
                    if (Internal.TryConvert(valueContainer, out arg))
                    {
                        Invoke(arg, owner);
                    }
                    else
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                }

                public void MaybeUnregisterCancelation() { }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueVoidVoid : IDelegateContinue
            {
                private readonly Action<ResultContainer> _callback;

                public DelegateContinueVoidVoid(Action<ResultContainer> callback)
                {
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke(new ResultContainer(valueContainer));
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueVoidResult<TResult> : IDelegateContinue
            {
                private readonly Func<ResultContainer, TResult> _callback;

                public DelegateContinueVoidResult(Func<ResultContainer, TResult> callback)
                {
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke(new ResultContainer(valueContainer));
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueArgVoid<TArg> : IDelegateContinue
            {
                private readonly Action<Promise<TArg>.ResultContainer> _callback;

                public DelegateContinueArgVoid(Action<Promise<TArg>.ResultContainer> callback)
                {
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke(new Promise<TArg>.ResultContainer(valueContainer));
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueArgResult<TArg, TResult> : IDelegateContinue
            {
                private readonly Func<Promise<TArg>.ResultContainer, TResult> _callback;

                public DelegateContinueArgResult(Func<Promise<TArg>.ResultContainer, TResult> callback)
                {
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke(new Promise<TArg>.ResultContainer(valueContainer));
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueVoidPromise : IDelegateContinuePromise
            {
                private readonly Func<ResultContainer, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueVoidPromise(Func<ResultContainer, Promise> callback)
                {
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Promise result = _callback.Invoke(new ResultContainer(valueContainer));
                    ((PromiseWaitPromise) owner).WaitFor(result);
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueVoidPromiseT<TPromise> : IDelegateContinuePromise
            {
                private readonly Func<ResultContainer, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueVoidPromiseT(Func<ResultContainer, Promise<TPromise>> callback)
                {
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Promise<TPromise> result = _callback.Invoke(new ResultContainer(valueContainer));
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(result);
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueArgPromise<TArg> : IDelegateContinuePromise
            {
                private readonly Func<Promise<TArg>.ResultContainer, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueArgPromise(Func<Promise<TArg>.ResultContainer, Promise> callback)
                {
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Promise result = _callback.Invoke(new Promise<TArg>.ResultContainer(valueContainer));
                    ((PromiseWaitPromise) owner).WaitFor(result);
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueArgPromiseT<TArg, TPromise> : IDelegateContinuePromise
            {
                private readonly Func<Promise<TArg>.ResultContainer, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueArgPromiseT(Func<Promise<TArg>.ResultContainer, Promise<TPromise>> callback)
                {
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Promise<TPromise> result = _callback.Invoke(new Promise<TArg>.ResultContainer(valueContainer));
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(result);
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }
            #endregion

            #region Delegates with capture value
#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            public sealed class FinallyDelegateCapture<TCapture> : Internal.ITreeHandleable, Internal.ITraceable
            {
#if PROMISE_DEBUG
                Internal.CausalityTrace Internal.ITraceable.Trace { get; set; }
#endif
                Internal.ITreeHandleable ILinked<Internal.ITreeHandleable>.Next { get; set; }

                private static ValueLinkedStack<Internal.ITreeHandleable> _pool;

                private TCapture _capturedValue;
                private Action<TCapture> _onFinally;

                private FinallyDelegateCapture() { }

                static FinallyDelegateCapture()
                {
                    Internal.OnClearPool += () => _pool.Clear();
                }

                public static FinallyDelegateCapture<TCapture> GetOrCreate(ref TCapture capturedValue, Action<TCapture> onFinally)
                {
                    var del = _pool.IsNotEmpty ? (FinallyDelegateCapture<TCapture>) _pool.Pop() : new FinallyDelegateCapture<TCapture>();
                    del._capturedValue = capturedValue;
                    del._onFinally = onFinally;
                    SetCreatedStacktrace(del, 2);
                    return del;
                }

                private void InvokeAndCatchAndDispose()
                {
                    var value = _capturedValue;
                    var callback = _onFinally;
                    SetCurrentInvoker(this);
                    Dispose();
                    try
                    {
                        callback.Invoke(value);
                    }
                    catch (Exception e)
                    {
                        Internal.AddRejectionToUnhandledStack(e, this);
                    }
                    ClearCurrentInvoker();
                }

                void Dispose()
                {
                    _capturedValue = default(TCapture);
                    _onFinally = null;
                    if (Config.ObjectPooling != PoolType.None)
                    {
                        _pool.Push(this);
                    }
                }

                void Internal.ITreeHandleable.Handle()
                {
                    InvokeAndCatchAndDispose();
                }

                void Internal.ITreeHandleable.MakeReady(Promise owner, Internal.IValueContainer valueContainer, ref ValueLinkedQueue<Internal.ITreeHandleable> handleQueue)
                {
                    handleQueue.Push(this);
                }

                void Internal.ITreeHandleable.MakeReadyFromSettled(Promise owner, Internal.IValueContainer valueContainer)
                {
                    Internal.AddToHandleQueueBack(this);
                }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureVoidVoid<TCapture> : IDelegateResolve, IDelegateReject, IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly TCapture _capturedValue;
                private readonly Action<TCapture> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureVoidVoid(ref TCapture capturedValue, Action<TCapture> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke(_capturedValue);
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke(_capturedValue);
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureArgVoid<TCapture, TArg> : IDelegateResolve, IDelegateReject, IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly TCapture _capturedValue;
                private readonly Action<TCapture, TArg> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureArgVoid(ref TCapture capturedValue, Action<TCapture, TArg> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                private void Invoke(TArg arg, Promise owner)
                {
                    _callback.Invoke(_capturedValue, arg);
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value, owner);
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg;
                    if (Internal.TryConvert(valueContainer, out arg))
                    {
                        Invoke(arg, owner);
                    }
                    else
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureVoidResult<TCapture, TResult> : IDelegateResolve, IDelegateReject, IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, TResult> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureVoidResult(ref TCapture capturedValue, Func<TCapture, TResult> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke(_capturedValue);
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke(_capturedValue);
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureArgResult<TCapture, TArg, TResult> : IDelegateResolve, IDelegateReject, IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, TArg, TResult> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureArgResult(ref TCapture capturedValue, Func<TCapture, TArg, TResult> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                private void Invoke(TArg arg, Promise owner)
                {
                    TResult result = _callback.Invoke(_capturedValue, arg);
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value, owner);
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg;
                    if (Internal.TryConvert(valueContainer, out arg))
                    {
                        Invoke(arg, owner);
                    }
                    else
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                }

                public void MaybeUnregisterCancelation() { }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureVoidPromise<TCapture> : IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureVoidPromise(ref TCapture capturedValue, Func<TCapture, Promise> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke(_capturedValue));
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke(_capturedValue));
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureArgPromise<TCapture, TArg> : IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, TArg, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureArgPromise(ref TCapture capturedValue, Func<TCapture, TArg, Promise> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                private void Invoke(TArg arg, Promise owner)
                {
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke(_capturedValue, arg));
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value, owner);
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg;
                    if (Internal.TryConvert(valueContainer, out arg))
                    {
                        Invoke(arg, owner);
                    }
                    else
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureVoidPromiseT<TCapture, TPromise> : IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly TCapture _capturedValue;
                public readonly Func<TCapture, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureVoidPromiseT(ref TCapture capturedValue, Func<TCapture, Promise<TPromise>> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke(_capturedValue));
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke(_capturedValue));
                }

                public void MaybeUnregisterCancelation() { }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureArgPromiseT<TCapture, TArg, TPromise> : IDelegateResolvePromise, IDelegateRejectPromise
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, TArg, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureArgPromiseT(ref TCapture capturedValue, Func<TCapture, TArg, Promise<TPromise>> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                private void Invoke(TArg arg, Promise owner)
                {
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke(_capturedValue, arg));
                }

                void IDelegateResolvePromise.InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value, owner);
                }

                public void InvokeRejecter(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg;
                    if (Internal.TryConvert(valueContainer, out arg))
                    {
                        Invoke(arg, owner);
                    }
                    else
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                }

                public void MaybeUnregisterCancelation() { }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureVoidVoid<TCapture> : IDelegateContinue
            {
                private readonly TCapture _capturedValue;
                private readonly Action<TCapture, ResultContainer> _callback;

                public DelegateContinueCaptureVoidVoid(ref TCapture capturedValue, Action<TCapture, ResultContainer> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke(_capturedValue, new ResultContainer(valueContainer));
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureVoidResult<TCapture, TResult> : IDelegateContinue
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, ResultContainer, TResult> _callback;

                public DelegateContinueCaptureVoidResult(ref TCapture capturedValue, Func<TCapture, ResultContainer, TResult> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke(_capturedValue, new ResultContainer(valueContainer));
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureArgVoid<TCapture, TArg> : IDelegateContinue
            {
                private readonly TCapture _capturedValue;
                private readonly Action<TCapture, Promise<TArg>.ResultContainer> _callback;

                public DelegateContinueCaptureArgVoid(ref TCapture capturedValue, Action<TCapture, Promise<TArg>.ResultContainer> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke(_capturedValue, new Promise<TArg>.ResultContainer(valueContainer));
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureArgResult<TCapture, TArg, TResult> : IDelegateContinue
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, Promise<TArg>.ResultContainer, TResult> _callback;

                public DelegateContinueCaptureArgResult(ref TCapture capturedValue, Func<TCapture, Promise<TArg>.ResultContainer, TResult> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke(_capturedValue, new Promise<TArg>.ResultContainer(valueContainer));
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureVoidPromise<TCapture> : IDelegateContinuePromise
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, ResultContainer, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureVoidPromise(ref TCapture capturedValue, Func<TCapture, ResultContainer, Promise> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Promise result = _callback.Invoke(_capturedValue, new ResultContainer(valueContainer));
                    ((PromiseWaitPromise) owner).WaitFor(result);
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureVoidPromiseT<TCapture, TPromise> : IDelegateContinuePromise
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, ResultContainer, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureVoidPromiseT(ref TCapture capturedValue, Func<TCapture, ResultContainer, Promise<TPromise>> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Promise<TPromise> result = _callback.Invoke(_capturedValue, new ResultContainer(valueContainer));
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(result);
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureArgPromise<TCapture, TArg> : IDelegateContinuePromise
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, Promise<TArg>.ResultContainer, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureArgPromise(ref TCapture capturedValue, Func<TCapture, Promise<TArg>.ResultContainer, Promise> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Promise result = _callback.Invoke(_capturedValue, new Promise<TArg>.ResultContainer(valueContainer));
                    ((PromiseWaitPromise) owner).WaitFor(result);
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureArgPromiseT<TCapture, TArg, TPromise> : IDelegateContinuePromise
            {
                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, Promise<TArg>.ResultContainer, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureArgPromiseT(ref TCapture capturedValue, Func<TCapture, Promise<TArg>.ResultContainer, Promise<TPromise>> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    Promise<TPromise> result = _callback.Invoke(_capturedValue, new Promise<TArg>.ResultContainer(valueContainer));
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(result);
                }

                public void CancelCallback() { throw new System.InvalidOperationException(); }
            }
            #endregion

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateResolvePassthroughCancel : IDelegateResolve, IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly bool _isActive;

                public bool IsNull { get { return !_isActive; } }

                public DelegateResolvePassthroughCancel(bool isActive)
                {
                    _isActive = isActive;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    owner.ResolveInternal(valueContainer);
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

            #region Delegates with cancelation token
#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateVoidVoidCancel : IDelegateResolve, IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Action _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateVoidVoidCancel(Action callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke();
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateArgVoidCancel<TArg> : IDelegateResolve, IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Action<TArg> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateArgVoidCancel(Action<TArg> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value);
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateVoidResultCancel<TResult> : IDelegateResolve, IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<TResult> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateVoidResultCancel(Func<TResult> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke();
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateArgResultCancel<TArg, TResult> : IDelegateResolve, IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<TArg, TResult> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateArgResultCancel(Func<TArg, TResult> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke(((Internal.ResolveContainer<TArg>) valueContainer).value);
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateVoidPromiseCancel : IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateVoidPromiseCancel(Func<Promise> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke());
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateArgPromiseCancel<TArg> : IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<TArg, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateArgPromiseCancel(Func<TArg, Promise> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg = ((Internal.ResolveContainer<TArg>) valueContainer).value;
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke(arg));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateVoidPromiseTCancel<TPromise> : IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateVoidPromiseTCancel(Func<Promise<TPromise>> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke());
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateArgPromiseTCancel<TArg, TPromise> : IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<TArg, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateArgPromiseTCancel(Func<TArg, Promise<TPromise>> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg = ((Internal.ResolveContainer<TArg>) valueContainer).value;
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke(arg));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueVoidVoidCancel : IDelegateContinue
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Action<ResultContainer> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueVoidVoidCancel(Action<ResultContainer> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        _callback.Invoke(new ResultContainer(valueContainer));
                        owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueVoidResultCancel<TResult> : IDelegateContinue
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<ResultContainer, TResult> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueVoidResultCancel(Func<ResultContainer, TResult> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        TResult result = _callback.Invoke(new ResultContainer(valueContainer));
                        owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueArgVoidCancel<TArg> : IDelegateContinue
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Action<Promise<TArg>.ResultContainer> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueArgVoidCancel(Action<Promise<TArg>.ResultContainer> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        _callback.Invoke(new Promise<TArg>.ResultContainer(valueContainer));
                        owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                    }
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueArgResultCancel<TArg, TResult> : IDelegateContinue
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<Promise<TArg>.ResultContainer, TResult> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueArgResultCancel(Func<Promise<TArg>.ResultContainer, TResult> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        TResult result = _callback.Invoke(new Promise<TArg>.ResultContainer(valueContainer));
                        owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueVoidPromiseCancel : IDelegateContinuePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<ResultContainer, Promise> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueVoidPromiseCancel(Func<ResultContainer, Promise> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        Promise result = _callback.Invoke(new ResultContainer(valueContainer));
                        ((PromiseWaitPromise) owner).WaitFor(result);
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueVoidPromiseTCancel<TPromise> : IDelegateContinuePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<ResultContainer, Promise<TPromise>> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueVoidPromiseTCancel(Func<ResultContainer, Promise<TPromise>> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        Promise<TPromise> result = _callback.Invoke(new ResultContainer(valueContainer));
                        ((PromiseWaitPromise<TPromise>) owner).WaitFor(result);
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueArgPromiseCancel<TArg> : IDelegateContinuePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<Promise<TArg>.ResultContainer, Promise> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueArgPromiseCancel(Func<Promise<TArg>.ResultContainer, Promise> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        Promise result = _callback.Invoke(new Promise<TArg>.ResultContainer(valueContainer));
                        ((PromiseWaitPromise) owner).WaitFor(result);
                    }
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueArgPromiseTCancel<TArg, TPromise> : IDelegateContinuePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly Func<Promise<TArg>.ResultContainer, Promise<TPromise>> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueArgPromiseTCancel(Func<Promise<TArg>.ResultContainer, Promise<TPromise>> callback)
                {
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        Promise<TPromise> result = _callback.Invoke(new Promise<TArg>.ResultContainer(valueContainer));
                        ((PromiseWaitPromise<TPromise>) owner).WaitFor(result);
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }
            #endregion

            #region Delegates with capture value and cancelation token
#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureVoidVoidCancel<TCapture> : IDelegateResolve, IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Action<TCapture> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureVoidVoidCancel(ref TCapture capturedValue, Action<TCapture> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke(_capturedValue);
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureArgVoidCancel<TCapture, TArg> : IDelegateResolve, IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Action<TCapture, TArg> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureArgVoidCancel(ref TCapture capturedValue, Action<TCapture, TArg> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    _callback.Invoke(_capturedValue, ((Internal.ResolveContainer<TArg>) valueContainer).value);
                    owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureVoidResultCancel<TCapture, TResult> : IDelegateResolve, IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, TResult> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureVoidResultCancel(ref TCapture capturedValue, Func<TCapture, TResult> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke(_capturedValue);
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureArgResultCancel<TCapture, TArg, TResult> : IDelegateResolve, IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, TArg, TResult> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureArgResultCancel(ref TCapture capturedValue, Func<TCapture, TArg, TResult> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TResult result = _callback.Invoke(_capturedValue, ((Internal.ResolveContainer<TArg>) valueContainer).value);
                    owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureVoidPromiseCancel<TCapture> : IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureVoidPromiseCancel(ref TCapture capturedValue, Func<TCapture, Promise> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke(_capturedValue));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureArgPromiseCancel<TCapture, TArg> : IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, TArg, Promise> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureArgPromiseCancel(ref TCapture capturedValue, Func<TCapture, TArg, Promise> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg = ((Internal.ResolveContainer<TArg>) valueContainer).value;
                    ((PromiseWaitPromise) owner).WaitFor(_callback.Invoke(_capturedValue, arg));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureVoidPromiseTCancel<TCapture, TPromise> : IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureVoidPromiseTCancel(ref TCapture capturedValue, Func<TCapture, Promise<TPromise>> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke(_capturedValue));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateCaptureArgPromiseTCancel<TCapture, TArg, TPromise> : IDelegateResolvePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, TArg, Promise<TPromise>> _callback;

                public bool IsNull { get { return _callback == null; } }

                public DelegateCaptureArgPromiseTCancel(ref TCapture capturedValue, Func<TCapture, TArg, Promise<TPromise>> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                }

                public void InvokeResolver(Internal.IValueContainer valueContainer, Promise owner)
                {
                    TArg arg = ((Internal.ResolveContainer<TArg>) valueContainer).value;
                    ((PromiseWaitPromise<TPromise>) owner).WaitFor(_callback.Invoke(_capturedValue, arg));
                }

                public void MaybeUnregisterCancelation()
                {
                    cancelationRegistration.TryUnregister();
                }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureVoidVoidCancel<TCapture> : IDelegateContinue
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Action<TCapture, ResultContainer> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureVoidVoidCancel(ref TCapture capturedValue, Action<TCapture, ResultContainer> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        _callback.Invoke(_capturedValue, new ResultContainer(valueContainer));
                        owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureVoidResultCancel<TCapture, TResult> : IDelegateContinue
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, ResultContainer, TResult> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureVoidResultCancel(ref TCapture capturedValue, Func<TCapture, ResultContainer, TResult> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        TResult result = _callback.Invoke(_capturedValue, new ResultContainer(valueContainer));
                        owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureArgVoidCancel<TCapture, TArg> : IDelegateContinue
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Action<TCapture, Promise<TArg>.ResultContainer> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureArgVoidCancel(ref TCapture capturedValue, Action<TCapture, Promise<TArg>.ResultContainer> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        _callback.Invoke(_capturedValue, new Promise<TArg>.ResultContainer(valueContainer));
                        owner.ResolveInternal(Internal.ResolveContainerVoid.GetOrCreate());
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureArgResultCancel<TCapture, TArg, TResult> : IDelegateContinue
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, Promise<TArg>.ResultContainer, TResult> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureArgResultCancel(ref TCapture capturedValue, Func<TCapture, Promise<TArg>.ResultContainer, TResult> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        TResult result = _callback.Invoke(_capturedValue, new Promise<TArg>.ResultContainer(valueContainer));
                        owner.ResolveInternal(Internal.ResolveContainer<TResult>.GetOrCreate(ref result));
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }


#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureVoidPromiseCancel<TCapture> : IDelegateContinuePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, ResultContainer, Promise> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureVoidPromiseCancel(ref TCapture capturedValue, Func<TCapture, ResultContainer, Promise> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        Promise result = _callback.Invoke(_capturedValue, new ResultContainer(valueContainer));
                        ((PromiseWaitPromise) owner).WaitFor(result);
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureVoidPromiseTCancel<TCapture, TPromise> : IDelegateContinuePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, ResultContainer, Promise<TPromise>> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureVoidPromiseTCancel(ref TCapture capturedValue, Func<TCapture, ResultContainer, Promise<TPromise>> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        Promise<TPromise> result = _callback.Invoke(_capturedValue, new ResultContainer(valueContainer));
                        ((PromiseWaitPromise<TPromise>) owner).WaitFor(result);
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureArgPromiseCancel<TCapture, TArg> : IDelegateContinuePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, Promise<TArg>.ResultContainer, Promise> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureArgPromiseCancel(ref TCapture capturedValue, Func<TCapture, Promise<TArg>.ResultContainer, Promise> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        Promise result = _callback.Invoke(_capturedValue, new Promise<TArg>.ResultContainer(valueContainer));
                        ((PromiseWaitPromise) owner).WaitFor(result);
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }

#if !PROTO_PROMISE_DEVELOPER_MODE
            [System.Diagnostics.DebuggerNonUserCode]
#endif
            internal struct DelegateContinueCaptureArgPromiseTCancel<TCapture, TArg, TPromise> : IDelegateContinuePromise
            {
                public CancelationRegistration cancelationRegistration;

                private readonly TCapture _capturedValue;
                private readonly Func<TCapture, Promise<TArg>.ResultContainer, Promise<TPromise>> _callback;
                private bool _canceled;

                public bool IsNull { get { return _callback == null; } }

                public DelegateContinueCaptureArgPromiseTCancel(ref TCapture capturedValue, Func<TCapture, Promise<TArg>.ResultContainer, Promise<TPromise>> callback)
                {
                    _capturedValue = capturedValue;
                    _callback = callback;
                    cancelationRegistration = default(CancelationRegistration);
                    _canceled = false;
                }

                public void Invoke(Internal.IValueContainer valueContainer, Promise owner)
                {
                    if (_canceled)
                    {
                        owner.RejectOrCancelInternal(valueContainer);
                    }
                    else
                    {
                        cancelationRegistration.TryUnregister();
                        Promise<TPromise> result = _callback.Invoke(_capturedValue, new Promise<TArg>.ResultContainer(valueContainer));
                        ((PromiseWaitPromise<TPromise>) owner).WaitFor(result);
                    }
                }

                public void CancelCallback()
                {
                    _canceled = true;
                }
            }
            #endregion
        }
    }
}