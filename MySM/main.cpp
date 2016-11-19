#include <string.h> //for strlen
#include <stdio.h>
#include <string>
#define XP_WIN
#include "main.h"
#include "js/jsapi.h"

using namespace std;


extern "C" _declspec(dllexport) const char* runJS(char* jsCode)
{
	JSRuntime *runtime = NULL;
	/* pointer to our context */
	JSContext *context = NULL;
	/* pointer to our global JavaScript object */
	JSObject *global = NULL;


	/* JavaScript value to store the result of the script */
	jsval rval;

	/* create new runtime, new context, global object */
	if ((!(runtime = JS_NewRuntime(8L * 1024L * 1024L)))
		|| (!(context = JS_NewContext(runtime, 8192)))
		|| (!(global = JS_NewObject(context, NULL, NULL, NULL)))
		)
		return "runtime初始化出错";
	/* set global object of context and initialize standard ECMAScript
	*      objects (Math, Date, ...) within this global object scope */
	if (!JS_InitStandardClasses(context, global))
		return "上下文初始化出错";

	/* now we are ready to run our script */
	/*
	if (!JS_EvaluateScript(context, global, script, strlen(script), "script", 1, &rval))
	return EXIT_FAILURE;

	printf("the script's result is: %d\n",JSVAL_TO_INT(rval));
	*/

	char buf[102400 * 5];
	JSString* jss;
	strcpy(buf, jsCode);
	int len = strlen(jsCode);
	if (len <= 0)return 0;

	JS_EvaluateScript(context, global, buf, len, "script", 1, &rval);
	jss = JS_ValueToString(context, rval);
	string jsRes = JS_GetStringBytes(jss);


	/* clean up */
	JS_DestroyContext(context);
	JS_DestroyRuntime(runtime);
	JS_ShutDown();
	string res = "The result is: " + jsRes;
	char szStr[1024];
	strncpy(szStr, res.c_str(), sizeof(szStr) - 1);
	return szStr;
}