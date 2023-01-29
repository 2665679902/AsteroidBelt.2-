using AsTool.Assert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AsTool.Unity.Component.UIComponent.DefaultUIComponent
{
    /// <summary>
    /// 一个用于管理<see cref="Image"/>的组件
    /// </summary>
    public class AsImage : AsMonoBehaviour
    {
        /// <summary>
        /// 内部管理的组件
        /// </summary>
        protected Image Image { get => GetComponent<Image>(); }

        /// <summary>
        /// 通过<see cref="Texture2D"/>设置图片
        /// </summary>
        /// <param name="texture">要设置的图片</param>
        public void SetImgae(Texture2D texture)
        {
            AsAssert.NotNull(texture, "texture can not be null");

            SetImgae(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
        }

        /// <summary>
        /// 通过<see cref="Sprite"/>设置图片
        /// </summary>
        /// <param name="sprite">要设置的图片</param>
        public void SetImgae(Sprite sprite)
        {
            AsAssert.NotNull(sprite, "sprite can not be null");

            if (sprite is null)
                return;

            Image.sprite = sprite;
        }
    }
}
