char* cStringCopy(const char* string)
{
   if (string == NULL)
   {
       return NULL;
   }
   char* res = (char*)malloc(strlen(string) + 1);
   strcpy(res, string);
   return res;
}

extern "C"
{
    char* GetIOSLocaleCode()
    {
        NSLocale *currentLocale = [NSLocale currentLocale];
        NSString * language = [currentLocale objectForKey:NSLocaleLanguageCode];
        return cStringCopy([language UTF8String]);
    }
    
    char* GetIOSCountryCode()
    {
        NSLocale *currentLocale = [NSLocale currentLocale];
        NSString *countryCode = [currentLocale objectForKey:NSLocaleCountryCode];
        return cStringCopy([countryCode UTF8String]);
    }
    
    bool GetIOSDeviceIsTablet()
    {
        return [[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPad;
    }
}
