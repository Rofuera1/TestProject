using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coroutines : MonoBehaviour
{
    public delegate float FunctionToLerp(float from, float to, float alpha);

    public static IEnumerator SetActiveAfterTime(GameObject obj, float time, bool active)
    {
        yield return new WaitForSeconds(time);

        if(obj != null)
            obj.SetActive(active);
    }

    public static IEnumerator StopEmittingAfterTime(ParticleSystem ps, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        ps.Stop();
    }

    public static IEnumerator LerpObjectParabolalike(Transform objectTR, Vector3 newPos, float time, FunctionToLerp smoothFunc, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);
        float ROTATION_SPEED = 10f;

        Vector3 startPos = objectTR.position;
        Vector3 prevPos = startPos;
        Vector3 currPos = startPos;

        Vector3 linearActualPosition = startPos;
        Vector3 linearMathPosition = startPos;
        Vector3 linearEndPosition = newPos;
        Vector3 perpendicularEndPosition = new Vector3(startPos.x, newPos.y, 0f);
        Vector3 perpendicularMathPosition = startPos;
        float yOffset = 0f;
        float xOffset = 0f;
        float lerpHalphAlpha = 0f;
        float alpha = 0f;
        float neededZDegree = 0f;

        float t = 0f;

        while(t < time)
        {
            t += Time.deltaTime;
            alpha = smoothFunc(0f, 1f, t / time);
            lerpHalphAlpha = alpha > 0.5f ? 1 - alpha : alpha;

            linearActualPosition = Vector3.Lerp(startPos, newPos, alpha);

            perpendicularMathPosition = Vector3.Lerp(startPos, perpendicularEndPosition, lerpHalphAlpha);
            linearMathPosition = Vector3.Lerp(startPos, linearEndPosition, lerpHalphAlpha);

            yOffset = (((linearMathPosition + perpendicularMathPosition) / 2f) - linearMathPosition).y;
            xOffset = (((linearMathPosition + perpendicularMathPosition) / 2f) - linearMathPosition).x;

            prevPos = currPos;
            currPos = new Vector3(linearActualPosition.x + xOffset, linearActualPosition.y + yOffset, 0f);

            objectTR.position = currPos;
            neededZDegree = (-Vector2.SignedAngle((currPos - prevPos), Vector3.up));
            objectTR.rotation = Quaternion.Lerp(objectTR.rotation, Quaternion.Euler(0f, 0f, neededZDegree), Time.deltaTime * ROTATION_SPEED);

            yield return null;
        }
    }

    public static IEnumerator LerpSliderFillAmountMultipleTimes(Image sliderImage, int timesAmount, float startValue, float newValue, float time, float delayTime, FunctionToLerp func, float timeToWait = 0f)
    {
        float valuePerStep = (newValue - startValue) / timesAmount;
        yield return new WaitForSeconds(timeToWait);
        for (int i = 0; i < timesAmount; i++)
        {
            float _startValue = startValue + valuePerStep * i;
            float _endValue = startValue + valuePerStep * (i + 1);
            float t = 0f;

            while (t < time)
            {
                t += Time.deltaTime;

                sliderImage.fillAmount = func(_startValue, _endValue, t / time);

                yield return null;
            }
            sliderImage.fillAmount = _endValue;
            yield return new WaitForSeconds(delayTime);
        }
    }

    public static IEnumerator ChangeTextAfterTime(Text text, string newText, float time)
    {
        yield return new WaitForSeconds(time);
        text.text = newText;
    }

    public static IEnumerator LerpSliderFillAmount(Image sliderImage, float newValue, float time, FunctionToLerp func, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);
        float startValue = sliderImage.fillAmount;
        float t = 0f;

        while(t < time)
        {
            t += Time.deltaTime;

            sliderImage.fillAmount = func(startValue, newValue, t / time);

            yield return null;
        }
    }

    public static IEnumerator LerpObjectMultipleRadiusParabalas(Transform objectTR, Vector2 endPos, int segmentsAmount, float fullTime, FunctionToLerp func, Camera camera, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);

        FunctionToLerp yOffsetFunc = EasingFunction.EaseOutQuart;
        Vector2 startPos = objectTR.position;
        Vector2 normalVector = (Quaternion.Euler(0f, 0f, 90f) * (endPos - startPos)).normalized;

        float startPercentage = 0f;
        float endPercentage = 0f;
        float screenWidth = camera.aspect * camera.orthographicSize;

        segmentsAmount = 1;

        float sign = endPos.x < objectTR.position.x ? 1 : -1;

        for (int i = 0; i < segmentsAmount; i++)
        {
            startPercentage = (float)i / (float)segmentsAmount;
            endPercentage = (float)(i + 1) / (float)segmentsAmount;

            Vector2 startSegmentPos = Vector2.Lerp(startPos, endPos, startPercentage);
            Vector2 endSegmentPos = Vector2.Lerp(startPos, endPos, endPercentage);
            Vector2 currentPosition = startSegmentPos;
            Vector2 previousPosition = startSegmentPos;
            float neededZDegree = 0f;
            float segmentLength = Vector2.Distance(startSegmentPos, endSegmentPos);
            float radius = segmentLength / 2;

            float t = 0f;
            float time = fullTime / segmentsAmount;

            float _x = 0f;
            float _y = 0f;
            float alpha = 0f;

            Vector3 edgePos = ((endSegmentPos + startSegmentPos) / 2) + normalVector * radius * sign;
            if (Mathf.Abs(edgePos.x) > screenWidth)
                sign /= 5;

            while (t < time)
            {
                t += Time.deltaTime;


                alpha = yOffsetFunc(0f, 1f, t / time);

                _x = Mathf.Lerp(-radius, radius, alpha);
                _y = Mathf.Sqrt((radius * radius) - (_x * _x)) * sign;

                previousPosition = currentPosition;
                currentPosition = Vector2.Lerp(startSegmentPos, endSegmentPos, alpha);
                currentPosition += normalVector * _y;
                objectTR.position = currentPosition;

                neededZDegree = (-Vector2.SignedAngle((currentPosition - previousPosition), Vector3.up));
                objectTR.rotation = Quaternion.Lerp(objectTR.rotation, Quaternion.Euler(0f, 0f, neededZDegree), Time.deltaTime * 15);

                yield return null;
            }
        }
    }

    public static IEnumerator LerpObjectMultipleRadiusParabalas(Transform objectTR, Vector2 endPos, int segmentsAmount, float fullTime, FunctionToLerp func, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);

        FunctionToLerp yOffsetFunc = EasingFunction.EaseOutQuart;
        Vector2 startPos = objectTR.position;
        Vector2 normalVector = (Quaternion.Euler(0f, 0f, 90f) * (endPos - startPos)).normalized;

        float startPercentage = 0f;
        float endPercentage = 0f;
        float screenWidth = Camera.main.aspect * Camera.main.orthographicSize;

        segmentsAmount = 1;

        float sign = endPos.x < objectTR.position.x ? 1 : -1;

        for (int i = 0; i < segmentsAmount; i++)
        {
            startPercentage = (float)i / (float)segmentsAmount;
            endPercentage = (float)(i + 1) / (float)segmentsAmount;

            Vector2 startSegmentPos = Vector2.Lerp(startPos, endPos, startPercentage);
            Vector2 endSegmentPos = Vector2.Lerp(startPos, endPos, endPercentage);
            Vector2 currentPosition = startSegmentPos;
            Vector2 previousPosition = startSegmentPos;
            float neededZDegree = 0f;
            float segmentLength = Vector2.Distance(startSegmentPos, endSegmentPos);
            float radius = segmentLength /2;

            float t = 0f;
            float time = fullTime / segmentsAmount;

            float _x = 0f;
            float _y = 0f;
            float alpha = 0f;

            Vector3 edgePos = ((endSegmentPos + startSegmentPos) / 2) + normalVector * radius * sign;
            if (Mathf.Abs(edgePos.x) > screenWidth)
                sign /= 5;

            while(t < time)
            {
                t += Time.deltaTime;


                alpha = yOffsetFunc(0f, 1f, t / time);

                _x = Mathf.Lerp(-radius , radius , alpha);
                _y = Mathf.Sqrt((radius * radius) - (_x * _x)) * sign;

                previousPosition = currentPosition;
                currentPosition = Vector2.Lerp(startSegmentPos, endSegmentPos, alpha);
                currentPosition += normalVector * _y;
                objectTR.position = currentPosition;

                neededZDegree = (-Vector2.SignedAngle((currentPosition - previousPosition), Vector3.up));
                objectTR.rotation = Quaternion.Lerp(objectTR.rotation, Quaternion.Euler(0f, 0f, neededZDegree), Time.deltaTime * 15);

                yield return null;
            }
        }
    }

    public static IEnumerator LerpObjectMultipleRadiusParabalasRocket(Transform objectTR, Vector2 endPos, float fullTime, FunctionToLerp func, float radius, float timeToWait = 0f, int segmentsAmount = 1)
    {
        yield return new WaitForSeconds(timeToWait);

        FunctionToLerp yOffsetFunc = EasingFunction.EaseOutQuart;
        Vector2 startPos = objectTR.position;
        Vector2 normalVector = (Quaternion.Euler(0f, 0f, 90f) * (endPos - startPos)).normalized;

        float startPercentage = 0f;
        float endPercentage = 0f;
        float screenWidth = Camera.main.aspect * Camera.main.orthographicSize;

        segmentsAmount = 1;

        for (int i = 0; i < segmentsAmount; i++)
        {
            startPercentage = (float)i / (float)segmentsAmount;
            endPercentage = (float)(i + 1) / (float)segmentsAmount;

            Vector2 startSegmentPos = Vector2.Lerp(startPos, endPos, startPercentage);
            Vector2 endSegmentPos = Vector2.Lerp(startPos, endPos, endPercentage);
            Vector2 currentPosition = startSegmentPos;
            Vector2 previousPosition = startSegmentPos;
            float neededZDegree = 0f;

            float t = 0f;
            float time = fullTime / segmentsAmount;

            float _x = 0f;
            float _y = 0f;
            float alpha = 0f;
            float sign = endPos.x < objectTR.position.x ? -1 : 1;

            Vector3 edgePos = ((endSegmentPos + startSegmentPos) / 2) + normalVector * radius * sign;
            if (Mathf.Abs(edgePos.x) > screenWidth)
                sign /= 5;

            while (t < time)
            {
                t += Time.deltaTime;
                alpha = yOffsetFunc(0f, 1f, t / time);

                _x = Mathf.Lerp(-radius, radius, alpha);
                _y = Mathf.Sqrt((radius * radius) - (_x * _x)) * sign;

                previousPosition = currentPosition;
                currentPosition = Vector2.Lerp(startSegmentPos, endSegmentPos, alpha);
                currentPosition += normalVector * _y;
                if(objectTR)  objectTR.position = currentPosition;

                neededZDegree = (-Vector2.SignedAngle((currentPosition - previousPosition), Vector3.up));
                if (objectTR) objectTR.rotation = Quaternion.Lerp(objectTR.rotation, Quaternion.Euler(0f, 0f, neededZDegree), Time.deltaTime * 15);

                yield return null;
            }
        }
    }


    public static IEnumerator LerpObjectMultipleRadiusParabalasOLD(Transform objectTR, Vector3 endPos, int segmentsAmount, float fullTime, FunctionToLerp func, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);
        float Y_OFFSET_MIN = 0.4f;
        float Y_OFFSET_MAX = 4f;
        float yOffsetCurrent = 0f;
        FunctionToLerp yOffsetFunc = EasingFunction.EaseOutQuart;

        float t = 0f;
        float alpha = 0f;
        float time = fullTime / segmentsAmount;
        Vector3 startPos = objectTR.position;
        float segmentLength = (endPos - startPos).magnitude / segmentsAmount;
        float neededZDegree = 0f;

        Vector3 previousPos = startPos;
        Vector3 linearPos = startPos;
        Vector3 linearSegmentStartPos = startPos;
        Vector3 linearSegmentEndPos = endPos;
        Vector3 linearVectorNormalized = (endPos - startPos).normalized;
        Vector3 linearNormalLeft = (Quaternion.Euler(0f, 0f, 90f) * (endPos - startPos)).normalized;

        Debug.DrawLine(startPos - Vector3.forward * 10, endPos, Color.red, 10f);

        for(int i = 0; i < segmentsAmount; i++)
        {
            t = 0f;
            linearSegmentStartPos = startPos + linearVectorNormalized * i * segmentLength;
            linearSegmentEndPos = startPos + linearVectorNormalized * (i + 1) * segmentLength;
            yOffsetCurrent = Random.Range(Y_OFFSET_MIN, Y_OFFSET_MAX);
            Debug.DrawLine(linearSegmentStartPos, linearSegmentStartPos + linearNormalLeft * yOffsetCurrent, Color.green, 10f);

            while (t < time)
            {
                t += Time.deltaTime;

                previousPos = linearPos;
                alpha = (t / time) < 0.5 ? yOffsetFunc(0f, 1f, t / (time / 2)) : yOffsetFunc(1f, 0f, (t - (time/2)) / (time / 2));
                linearPos = Vector3.Lerp(linearSegmentStartPos, linearSegmentEndPos, func(0f, 1f, t/time));
                linearPos += linearNormalLeft * (yOffsetCurrent * alpha);// * (i % 2 == 0? 1 : -1);

                objectTR.position = linearPos;

                neededZDegree = (-Vector2.SignedAngle((linearPos - previousPos), Vector3.up));
                objectTR.rotation = Quaternion.Lerp(objectTR.rotation, Quaternion.Euler(0f, 0f, neededZDegree), Time.deltaTime * 15);

                yield return null;
            }
        }
    }

    public static IEnumerator LerpObjectPositionRotation(Transform objectTR, Vector3 newPos, Quaternion newRotation, float time, FunctionToLerp smoothFunc, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);

        Vector3 startPos = objectTR.position;
        Quaternion startRot = objectTR.rotation;

        float t = 0f;
        float alpha = 0f;
        while(t < time)
        {
            t += Time.deltaTime;

            alpha = smoothFunc(0f, 1f, t / time);
            objectTR.position = Vector3.Lerp(startPos, newPos, alpha);
            objectTR.rotation = Quaternion.Lerp(startRot, newRotation, alpha);

            yield return null;
        }
    }

    public static IEnumerator LerpGameObjectPingPongScale(Transform objectTR, Vector3 startScale, Vector3 endScale, float halfTime, FunctionToLerp smoothFunc, float timeToWait = 0f)
    {
        yield return new WaitForSecondsRealtime(timeToWait);

        float t = 0f;
        objectTR.localScale = startScale;

        while (t < halfTime)
        {
            t += Time.unscaledDeltaTime;

            if (objectTR == null)
                break;

            objectTR.localScale = Vector3.Lerp(startScale, endScale, smoothFunc(0f, 1f, t / halfTime));
            
            yield return null;
        }

        t = 0f;

        while (t < halfTime)
        {
            t += Time.unscaledDeltaTime;

            if (objectTR == null)
                break;

            objectTR.localScale = Vector3.Lerp(endScale, startScale, smoothFunc(0f, 1f, t / halfTime));

            yield return null;
        }

        if (objectTR != null)
            objectTR.localScale = startScale;
    }

    public static IEnumerator Lerp360RotateYRealTime(Transform tr, float time, FunctionToLerp func, float timeToWait = 0f)
    {
        yield return new WaitForSecondsRealtime(timeToWait);

        float t = 0f;
        float speed = 360 / time;
        Quaternion startRot = tr.rotation;

        while(t < time)
        {
            t += Time.unscaledDeltaTime;

            tr.Rotate(Vector3.up, speed * Time.deltaTime);

            yield return null;
        }

        tr.rotation = startRot;
    }

    public static IEnumerator LerpTextColorPingPong(Text text, Color startColor, Color endColor, float halfTime, FunctionToLerp funcToUse, float timeToWait = 0f)
    {
        yield return new WaitForSecondsRealtime(timeToWait);

        float t = 0f;

        while (t < halfTime)
        {
            t += Time.unscaledDeltaTime;

            text.color = Color.Lerp(startColor, endColor, funcToUse(0f, 1f, t / halfTime));

            yield return null;
        }

        t = 0f;

        while (t < halfTime)
        {
            t += Time.unscaledDeltaTime;

            text.color = Color.Lerp(startColor, endColor, funcToUse(1f, 0f, t / halfTime));

            yield return null;
        }
    }

    public static IEnumerator LerpTransformScaleRealTime(Transform objectTR, Vector3 endScale, float time, FunctionToLerp smoothFunc, bool enabledOnEnd = true, float timeToWait = 0.0f)
    {
        yield return new WaitForSecondsRealtime(timeToWait);
        Vector3 startScale = objectTR.transform.localScale;
        float t = 0f;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;

            objectTR.transform.localScale = Vector3.Lerp(startScale, endScale, smoothFunc(0f, 1f, smoothFunc(0f, 1f, t / time)));

            yield return null;
        }

        objectTR.transform.localScale = endScale;
        objectTR.gameObject.SetActive(enabledOnEnd);
    }

    public static IEnumerator LerpTransformScale(Transform objectTR, Vector3 endScale, float time, FunctionToLerp smoothFunc, bool enabledOnEnd = true, float timeToWait = 0.0f)
    {
        yield return new WaitForSeconds(timeToWait);
        Vector3 startScale = objectTR.transform.localScale;
        float t = 0f;

        while(t < time)
        {
            t += Time.deltaTime;

            objectTR.transform.localScale = Vector3.Lerp(startScale, endScale, smoothFunc(0f, 1f, t / time));

            yield return null;
        }

        objectTR.transform.localScale = endScale;
        objectTR.gameObject.SetActive(enabledOnEnd);
    }

    public static IEnumerator LerpGameObjectPingPongLocalPosition(Transform gameObjectTR, Vector3 toPositionLocal, float halfTime, FunctionToLerp smoothFunc, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);
        float t = 0f;
        Vector3 fromPos = gameObjectTR.localPosition;

        while (t < halfTime)
        {
            t += Time.deltaTime;

            gameObjectTR.localPosition = Vector3.Lerp(fromPos, toPositionLocal, smoothFunc(0f, 1f, t / halfTime));

            yield return null;
        }

        t = 0f;

        while (t < halfTime)
        {
            t += Time.deltaTime;

            gameObjectTR.localPosition = Vector3.Lerp(toPositionLocal, fromPos, smoothFunc(0f, 1f, t / halfTime));

            yield return null;
        }

        gameObjectTR.localPosition = fromPos;
    }

    public static IEnumerator LerpGameObjectPingPong(Transform gameObjectTR, Vector3 toPosition, float halfTime, FunctionToLerp smoothFunc, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);
        float t = 0f;
        Vector3 fromPos = gameObjectTR.position;

        while(t < halfTime)
        {
            t += Time.deltaTime;

            gameObjectTR.position = Vector3.Lerp(fromPos, toPosition, smoothFunc(0f, 1f, t / halfTime));

            yield return null;
        }

        t = 0f;

        while (t < halfTime)
        {
            t += Time.deltaTime;

            gameObjectTR.position = Vector3.Lerp(toPosition, fromPos, smoothFunc(0f, 1f, t / halfTime));

            yield return null;
        }

        gameObjectTR.position = fromPos;
    }

    public static IEnumerator LerpImageFill(UnityEngine.UI.Image image, float toFill, float time, FunctionToLerp smoothFunction, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);
        float t = 0f;
        float startFill = image.fillAmount;

        while(t < time)
        {
            t += Time.deltaTime;

            image.fillAmount = smoothFunction(startFill, toFill, t / time);

            yield return null;
        }

        image.fillAmount = toFill;
    }

    public static IEnumerator LerpGameObjectFromTo_KeepRotation(Transform gameObjectTR, Vector3 toPosition, float timeToLerp, FunctionToLerp smoothFunction, float timeToWait = 0f, bool enabledAfter = true)
    {
        yield return new WaitForSeconds(timeToWait);

        float t = 0f;
        Vector3 fromPosition = gameObjectTR.position;

        Vector3 prevPos = fromPosition;
        Vector3 currPos = fromPosition;
        float neededZDegree = 0f;

        while (t < timeToLerp)
        {
            t += Time.deltaTime;

            prevPos = currPos;
            currPos = Vector3.Lerp(fromPosition, toPosition, smoothFunction(0f, 1f, t / timeToLerp));

            gameObjectTR.position = currPos;
            neededZDegree = (-Vector2.SignedAngle((currPos - prevPos), Vector3.up));
            gameObjectTR.rotation = Quaternion.Lerp(gameObjectTR.rotation, Quaternion.Euler(0f, 0f, neededZDegree), Time.deltaTime * 20);

            yield return null;
        }

        if (!gameObjectTR)
            yield break;

        gameObjectTR.position = toPosition;
        gameObjectTR.gameObject.SetActive(enabledAfter);
    }

    public static IEnumerator LerpObjectAnchoredPosition(RectTransform tr, Vector3 posTo, float timeLerp, FunctionToLerp func)
    {
        float t = 0f;
        Vector3 posFrom = tr.anchoredPosition;

        while(t < timeLerp)
        {
            t += Time.unscaledDeltaTime;

            tr.anchoredPosition = Vector2.Lerp(posFrom, posTo, func(0f, 1f, t / timeLerp));
            ///Debug.Log("FROM_" + posFrom + "_TO_" + posTo + "_NOW_" + tr.anchoredPosition);

            yield return null;
        }
    }

    public static IEnumerator LerpObjectShakeByX(Transform objToShake, float xRange, float time, FunctionToLerp func)
    {
        float t = 0f;
        float halfTime = time * 0.5f;
        Vector3 startPos = objToShake.position;
        Vector3 rightPos = startPos;
        Vector3 leftPos = startPos;
        rightPos.x += xRange;
        leftPos.x -= xRange;

        while(t < halfTime)
        {
            t += Time.deltaTime;

            objToShake.position = Vector3.Lerp(startPos, rightPos, func(0f, 1f, t / halfTime));

            yield return null;
        }

        t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            objToShake.position = Vector3.Lerp(rightPos, leftPos, func(0f, 1f, t / time));

            yield return null;
        }

        t = 0f;

        while (t < halfTime)
        {
            t += Time.deltaTime;

            objToShake.position = Vector3.Lerp(leftPos, startPos, func(0f, 1f, t / halfTime));

            yield return null;
        }
    }

    public static IEnumerator LerpGameObjectFromToPosition(Transform gameObjectTR, Vector3 toPosition, float timeToLerp, FunctionToLerp smoothFunction, float timeToWait = 0f, bool enabledAfter = true)
    {
        yield return new WaitForSeconds(timeToWait);

        float t = 0f;
        Vector3 fromPosition = gameObjectTR.position;

        while (t < timeToLerp)
        {
            t += Time.unscaledDeltaTime;

            if (!gameObjectTR)
                break;

            gameObjectTR.position = Vector3.Lerp(fromPosition, toPosition, smoothFunction(0f, 1f, t / timeToLerp));

            yield return null;
        }

        if (!gameObjectTR)
            yield break;

        gameObjectTR.position = toPosition;
        gameObjectTR.gameObject.SetActive(enabledAfter);
    }

    public static IEnumerator LerpGameObjectFromToLocalPositionRealTime(Transform gameObjectTR, Vector3 toLocalPosition, float timeToLerp, FunctionToLerp smoothFunction, float timeToWait = 0f, bool enabledAfter = true)
    {
        yield return new WaitForSecondsRealtime(timeToWait);

        float t = 0f;
        Vector3 fromPosition = gameObjectTR.localPosition;

        while (t < timeToLerp)
        {
            t += Time.unscaledDeltaTime;

            if (!gameObjectTR)
                break;

            gameObjectTR.localPosition = Vector3.Lerp(fromPosition, toLocalPosition, smoothFunction(0f, 1f, t / timeToLerp));

            yield return null;
        }

        if (!gameObjectTR)
            yield break;

        gameObjectTR.localPosition = toLocalPosition;
        gameObjectTR.gameObject.SetActive(enabledAfter);
    }

    public static IEnumerator LerpGameObjectFromToLocalPosition(Transform gameObjectTR, Vector3 toLocalPosition, float timeToLerp, FunctionToLerp smoothFunction, float timeToWait = 0f, bool enabledAfter = true)
    {
        yield return new WaitForSeconds(timeToWait);

        float t = 0f;
        Vector3 fromPosition = gameObjectTR.localPosition;

        while (t < timeToLerp)
        {
            t += Time.unscaledDeltaTime;

            if (!gameObjectTR)
                break;

            gameObjectTR.localPosition = Vector3.Lerp(fromPosition, toLocalPosition, smoothFunction(0f, 1f, t / timeToLerp));

            yield return null;
        }

        if (!gameObjectTR)
            yield break;

        gameObjectTR.localPosition = toLocalPosition;
        gameObjectTR.gameObject.SetActive(enabledAfter);
    }

    public static IEnumerator LerpGameObjectFromToRealTime(Transform gameObjectTR, Vector3 toPosition, float timeToLerp, FunctionToLerp smoothFunction, float timeToWait = 0f, bool enabledAfter = true)
    {
        yield return new WaitForSecondsRealtime(timeToWait);

        float t = 0f;
        Vector3 fromPosition = gameObjectTR.position;

        while (t < timeToLerp)
        {
            t += Time.unscaledDeltaTime;

            if (!gameObjectTR)
                break;

            gameObjectTR.position = Vector3.Lerp(fromPosition, toPosition, smoothFunction(0f, 1f, t / timeToLerp));

            yield return null;
        }

        if (!gameObjectTR)
            yield break;

        gameObjectTR.position = toPosition;
        gameObjectTR.gameObject.SetActive(enabledAfter);
    }

    public static IEnumerator LerpGameObjectFromTo(Transform gameObjectTR, Vector3 toPosition, float timeToLerp, FunctionToLerp smoothFunction, float timeToWait = 0f, bool enabledAfter = true)
    {
        yield return new WaitForSeconds(timeToWait);

        float t = 0f;
        Vector3 fromPosition = gameObjectTR.position;

        while (t < timeToLerp)
        {
            t += Time.deltaTime;

            if (!gameObjectTR)
                break;

            gameObjectTR.position = Vector3.Lerp(fromPosition, toPosition, smoothFunction(0f, 1f, t / timeToLerp));

            yield return null;
        }

        if (!gameObjectTR)
            yield break;

        gameObjectTR.position = toPosition;
        gameObjectTR.gameObject.SetActive(enabledAfter);
    }

    public static IEnumerator LerpCanvasGroupAlpha(CanvasGroup image, float endA, float time, bool interactableAtEnd, float timeToWait = 0f)
    {
        yield return new WaitForSecondsRealtime(timeToWait);

        float t = 0f;
        float startAlpha = image.alpha;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;

            image.alpha = Mathf.Lerp(startAlpha, endA, t / time);

            yield return null;
        }

        image.alpha = endA;
        image.blocksRaycasts = interactableAtEnd;
        image.interactable = interactableAtEnd;
    }

    public static IEnumerator LerpImageAlpha(UnityEngine.UI.Image image, float startA, float endA, float time, float timeToWait = 0f)
    {
        yield return new WaitForSecondsRealtime(timeToWait);

        float t = 0f;
        Color startColor = image.color;
        Color endColor = startColor;
        startColor.a = startA;
        endColor.a = endA;
        image.color = startColor;

        while(t < time)
        {
            t += Time.unscaledDeltaTime;

            image.color = Color.Lerp(startColor, endColor, t / time);

            yield return null;
        }

        image.color = endColor;
    }

    public static IEnumerator LerpTextAlpha(UnityEngine.UI.Text text, float startA, float endA, float time, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);

        float t = 0f;
        Color startColor = text.color;
        Color endColor = startColor;
        startColor.a = startA;
        endColor.a = endA;
        text.color = startColor;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;

            text.color = Color.Lerp(startColor, endColor, t / time);

            yield return null;
        }

        text.color = endColor;
    }

    public static IEnumerator LerpSpriteRendererAlpha(SpriteRenderer renderer, float startA, float endA, float time, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);

        float t = 0f;
        Color startColor = renderer.color;
        Color endColor = startColor;
        startColor.a = startA;
        endColor.a = endA;

        while (t < time)
        {
            t += Time.deltaTime;

            renderer.color = Color.Lerp(startColor, endColor, t / time);

            yield return null;
        }

        renderer.color = endColor;
    }

    public static IEnumerator LerpSpriteRendererAlpha(SpriteRenderer renderer, float time, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);

        float t = 0f;
        Color startColor = renderer.color;
        Color endColor = startColor;
        endColor.a = 0f;

        while(t < time)
        {
            t += Time.deltaTime;

            renderer.color = Color.Lerp(startColor, endColor, t / time);

            yield return null;
        }

        renderer.color = endColor;
    }

    public static IEnumerator LerpSpriteRendererColor(SpriteRenderer renderer, Color toColor, float time, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);
        float t = 0f;
        Color colorFrom = renderer.color;

        while(t < time)
        {
            t += Time.deltaTime;

            renderer.color = Color.Lerp(colorFrom, toColor, t / time);

            yield return null;
        }

        renderer.color = toColor;
    }
    
    public static IEnumerator LerpImageColor(Image renderer, Color toColor, float time, float timeToWait = 0f)
    {
        yield return new WaitForSeconds(timeToWait);
        float t = 0f;
        Color colorFrom = renderer.color;

        while(t < time)
        {
            t += Time.deltaTime;

            renderer.color = Color.Lerp(colorFrom, toColor, t / time);

            yield return null;
        }

        renderer.color = toColor;
    }
}
