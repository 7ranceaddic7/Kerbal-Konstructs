﻿using System;
using UnityEngine;
using KerbalKonstructs.API;

namespace KerbalKonstructs.LaunchSites
{
	public class LaunchSite
	{
		[PersistentKey]
		public string name;

		public string author;
		public SiteType type;
		public Texture logo;
		public Texture icon;
		public string description;

		public string category;
		public float opencost;
		public float closevalue;

		[PersistentField]
		public string openclosestate;

		[PersistentField]
		public string favouritesite;

		[PersistentField]
		public float missioncount;

		[PersistentField]
		public string missionlog;

		public float reflon;
		public float reflat;
		public float refalt;
		public float sitelength;
		public float sitewidth;
		public float launchrefund;
		public float recoveryfactor;
		public float recoveryrange;

		public string nation;

		public GameObject GameObject;
		public PSystemSetup.SpaceCenterFacility facility;

		public LaunchSite(string sName, string sAuthor, SiteType sType, Texture sLogo, Texture sIcon,
			string sDescription, string sDevice, float fOpenCost, float fCloseValue, string sOpenCloseState, float fRefLon, 
			float fRefLat, float fRefAlt, float fLength, float fWidth, float fRefund, float fRecoveryFactor, float fRecoveryRange,
			GameObject gameObject, PSystemSetup.SpaceCenterFacility newFacility = null, 
			string sMissionLog = "Too busy to keep this log. Signed Gene Kerman.", 
			string sNation = "United Kerbin", string sFavourite = "No", float fMissionCount = 0)
		{
			name = sName;
			author = sAuthor;
			type = sType;
			logo = sLogo;
			icon = sIcon;
			description = sDescription;
			category = sDevice;
			opencost = fOpenCost;
			closevalue = fCloseValue;
			openclosestate = sOpenCloseState;
			GameObject = gameObject;
			facility = newFacility;
			reflon = fRefLon;
			reflat = fRefLat;
			refalt = fRefAlt;
			sitelength = fLength;
			sitewidth = fWidth;
			launchrefund = fRefund;
			recoveryfactor = fRecoveryFactor;
			recoveryrange = fRecoveryRange;
			favouritesite = sFavourite;
			missioncount = fMissionCount;
			nation = sNation;
			missionlog = sMissionLog;
		}
	}

	public enum SiteType
	{
		VAB,
		SPH,
		Any
	}
}
