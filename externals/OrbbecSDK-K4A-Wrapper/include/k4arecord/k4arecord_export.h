
#ifndef K4ARECORD_EXPORT_H
#define K4ARECORD_EXPORT_H

#ifdef K4ARECORD_STATIC_DEFINE
#  define K4ARECORD_EXPORT
#  define K4ARECORD_NO_EXPORT
#else
#  ifndef K4ARECORD_EXPORT
#    ifdef k4arecord_EXPORTS
        /* We are building this library */
#      define K4ARECORD_EXPORT __declspec(dllexport)
#    else
        /* We are using this library */
#      define K4ARECORD_EXPORT __declspec(dllimport)
#    endif
#  endif

#  ifndef K4ARECORD_NO_EXPORT
#    define K4ARECORD_NO_EXPORT 
#  endif
#endif

#ifndef K4ARECORD_DEPRECATED
#  define K4ARECORD_DEPRECATED __declspec(deprecated)
#endif

#ifndef K4ARECORD_DEPRECATED_EXPORT
#  define K4ARECORD_DEPRECATED_EXPORT K4ARECORD_EXPORT K4ARECORD_DEPRECATED
#endif

#ifndef K4ARECORD_DEPRECATED_NO_EXPORT
#  define K4ARECORD_DEPRECATED_NO_EXPORT K4ARECORD_NO_EXPORT K4ARECORD_DEPRECATED
#endif

#if 0 /* DEFINE_NO_DEPRECATED */
#  ifndef K4ARECORD_NO_DEPRECATED
#    define K4ARECORD_NO_DEPRECATED
#  endif
#endif

#endif /* K4ARECORD_EXPORT_H */
