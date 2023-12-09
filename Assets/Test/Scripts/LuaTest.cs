using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaTest : MonoBehaviour
{
    private void Start()
    {
        XLua.LuaEnv luaenv = new XLua.LuaEnv();
        luaenv.DoString("CS.UnityEngine.Debug.Log('hello world')");
        luaenv.Dispose();
    }
}