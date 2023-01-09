/** \file k4abt.hpp
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 * Azure Kinect Body Tracking SDK - C++ wrapper.
 */

#ifndef K4ABT_HPP
#define K4ABT_HPP

#include <k4a/k4a.hpp>
#include "k4abt.h"

namespace k4abt
{

    /**
     * \defgroup cppsdk C++ Reference
     *
     * Functions part of the SDK.
     *
     * @{
     */

    /** \class frame k4abt.hpp <k4a/k4abt.hpp>
     * Wrapper for \ref k4abt_frame_t
     *
     * Wraps a handle for a k4abt frame.  Copying/moving is cheap, copies are shallow.
     *
     * \sa k4abt_frame_t
     */
    class frame
    {
    public:
        /** Creates a k4abt frame from a k4abt_frame_t.
         * Takes ownership of the handle, i.e. assuming that handle has a refcount
         * of 1, you should not call k4abt_frame_release on the handle after giving
         * it to the frame; the frame class will take care of that.
         */
        frame(k4abt_frame_t handle = nullptr) noexcept : m_handle(handle) {}

        /** Creates a shallow copy of another k4abt frame
         */
        frame(const frame &other) noexcept : m_handle(other.m_handle)
        {
            if (m_handle != nullptr)
            {
                k4abt_frame_reference(m_handle);
            }
        }

        /** Moves another k4abt frame into a new frame
         */
        frame(frame &&other) noexcept : m_handle(other.m_handle)
        {
            other.m_handle = nullptr;
        }

        ~frame()
        {
            reset();
        }

        /** Sets frame to a shallow copy of other
         */
        frame &operator=(const frame &other) noexcept
        {
            if (this != &other)
            {
                reset();
                m_handle = other.m_handle;
                if (m_handle != nullptr)
                {
                    k4abt_frame_reference(m_handle);
                }
            }
            return *this;
        }

        /** Moves another frame into this frame; other is set to invalid
         */
        frame &operator=(frame &&other) noexcept
        {
            if (this != &other)
            {
                reset();
                m_handle = other.m_handle;
                other.m_handle = nullptr;
            }
            return *this;
        }

        /** Invalidates this frame
         */
        frame &operator=(std::nullptr_t) noexcept
        {
            reset();
            return *this;
        }

        /** Returns true if two frames refer to the same k4abt_frame_t, false otherwise
         */
        bool operator==(const frame &other) const noexcept
        {
            return m_handle == other.m_handle;
        }

        /** Returns false if the frame is valid, true otherwise
         */
        bool operator==(std::nullptr_t) const noexcept
        {
            return m_handle == nullptr;
        }

        /** Returns true if two frames wrap different k4abt_frame_t instances, false otherwise
         */
        bool operator!=(const frame &other) const noexcept
        {
            return m_handle != other.m_handle;
        }

        /** Returns true if the frame is valid, false otherwise
         */
        bool operator!=(std::nullptr_t) const noexcept
        {
            return m_handle != nullptr;
        }

        /** Returns true if the frame is valid, false otherwise
         */
        operator bool() const noexcept
        {
            return m_handle != nullptr;
        }

        /** Returns the underlying k4abt_frame_t handle
         *
         * Note that this function does not increment the reference count on the k4abt_frame_t.
         * The caller is responsible for incrementing the reference count on
         * the k4abt_frame_t if the caller needs the k4abt_frame_t to outlive this C++ object.
         * Otherwise, the k4abt_frame_t will be destroyed by this C++ object.
         */
        k4abt_frame_t handle() const noexcept
        {
            return m_handle;
        }

        /** Releases the underlying k4abt_frame_t; the frame is set to invalid.
         */
        void reset() noexcept
        {
            if (m_handle != nullptr)
            {
                k4abt_frame_release(m_handle);
                m_handle = nullptr;
            }
        }

        /** Get the number of people detected from the k4abt frame
         *
         * \sa k4abt_frame_get_num_bodies
         */
        uint32_t get_num_bodies() const noexcept
        {
            return k4abt_frame_get_num_bodies(m_handle);
        }

        /** Get the joint information for a particular person index from the k4abt_frame_t
         *
         * \sa k4abt_frame_get_body_skeleton
         */
        k4abt_skeleton_t get_body_skeleton(uint32_t index) const
        {
            k4abt_skeleton_t skeleton;
            get_body_skeleton(index, skeleton);
            return skeleton;
        }

        /** Get the joint information for a particular person index from the k4abt_frame_t
         *
         * \sa k4abt_frame_get_body_skeleton
         */
        void get_body_skeleton(uint32_t index, k4abt_skeleton_t& skeleton) const
        {
            k4a_result_t result = k4abt_frame_get_body_skeleton(m_handle, index, &skeleton);
            if (K4A_RESULT_SUCCEEDED != result)
            {
                throw k4a::error("Get body skeleton failed!");
            }
        }

        /** Get the body id for a particular person index from the k4abt frame
         *
         * \sa k4abt_frame_get_body_id
         */
        uint32_t get_body_id(uint32_t index) const noexcept
        {
            return k4abt_frame_get_body_id(m_handle, index);
        }

        /** Get the full body struct for a particular person index from the k4abt frame
         *
         * \sa k4abt_frame_get_body_id
         * \sa k4abt_frame_get_body_skeleton
         */
        k4abt_body_t get_body(uint32_t index) const
        {
            k4abt_body_t body;
            body.id = k4abt_frame_get_body_id(m_handle, index);
            get_body_skeleton(index, body.skeleton);
            return body;
        }

        /** Get the k4abt frame's device timestamp in microseconds
         *
         * \sa k4a_image_get_device_timestamp_usec
         */
        std::chrono::microseconds get_device_timestamp() const noexcept
        {
            return std::chrono::microseconds(k4abt_frame_get_device_timestamp_usec(m_handle));
        }

        /** Get the k4abt frame's system timestamp in nanoseconds
         *
         * \sa k4a_image_get_system_timestamp_nsec
         */
        std::chrono::nanoseconds get_system_timestamp() const noexcept
        {
            return std::chrono::nanoseconds(k4abt_frame_get_system_timestamp_nsec(m_handle));
        }

        /** Get the body index map associated with the k4abt frame
         *
         * \sa k4abt_frame_get_body_index_map
         */
        k4a::image get_body_index_map() const noexcept
        {
            return k4a::image(k4abt_frame_get_body_index_map(m_handle));
        }

        /** Get the original capture that is used to calculate the k4abt frame
         *
         * \sa k4abt_frame_get_capture
         */
        k4a::capture get_capture() const noexcept
        {
            return k4a::capture(k4abt_frame_get_capture(m_handle));
        }

    private:
        k4abt_frame_t m_handle;
    };

    /** \class tracker k4abt.hpp <k4a/k4abt.hpp>
     * Wrapper for \ref k4abt_tracker_t
     *
     * Wraps a handle for a k4abt tracker.
     */
    class tracker
    {
    public:
        /** Creates a tracker from a k4abt_tracker_t
         * Takes ownership of the handle, i.e. you should not call
         * k4abt_tracker_destroy on the handle after giving it to the
         * tracker; the tracker will take care of that.
         */
        tracker(k4abt_tracker_t handle = nullptr) noexcept : m_handle(handle) {}

        /** Moves another tracker into a new tracker
         */
        tracker(tracker &&dev) noexcept : m_handle(dev.m_handle)
        {
            dev.m_handle = nullptr;
        }

        tracker(const tracker &) = delete;

        ~tracker()
        {
            destroy();
        }

        tracker &operator=(const tracker &) = delete;

        /** Moves another tracker into this tracker; other is set to invalid
         */
        tracker &operator=(tracker &&dev) noexcept
        {
            if (this != &dev)
            {
                destroy();
                m_handle = dev.m_handle;
                dev.m_handle = nullptr;
            }
            return *this;
        }

        /** Returns true if the tracker is valid, false otherwise
         */
        operator bool() const noexcept
        {
            return m_handle != nullptr;
        }

        /** Destroys a k4abt tracker.
         *
         * \sa k4abt_tracker_destroy
         */
        void destroy() noexcept
        {
            if (m_handle != nullptr)
            {
                k4abt_tracker_destroy(m_handle);
                m_handle = nullptr;
            }
        }

        /** Add a k4a sensor capture to the tracker input queue to generate its body tracking result asynchronously.
         * Throws error on failure.
         *
         * \sa k4abt_tracker_enqueue_capture
         */
        bool enqueue_capture(k4a::capture cap, std::chrono::milliseconds timeout = std::chrono::milliseconds(K4A_WAIT_INFINITE))
        {
            int32_t timeout_ms = k4a::internal::clamp_cast<int32_t>(timeout.count());
            k4a_wait_result_t result = k4abt_tracker_enqueue_capture(m_handle, cap.handle(), timeout_ms);
            if (result == K4A_WAIT_RESULT_FAILED)
            {
                throw k4a::error("Failed to enqueue capture to tracker!");
            }
            else if (result == K4A_WAIT_RESULT_TIMEOUT)
            {
                return false;
            }

            return true;
        }

        /** Gets the next available body frame.
         * Throws error on failure.
         *
         * \sa k4abt_tracker_pop_result
         */
        bool pop_result(k4abt::frame* body_frame, std::chrono::milliseconds timeout = std::chrono::milliseconds(K4A_WAIT_INFINITE)) const
        {
            k4abt_frame_t frame_handle = nullptr;
            int32_t timeout_ms = k4a::internal::clamp_cast<int32_t>(timeout.count());
            k4a_wait_result_t result = k4abt_tracker_pop_result(m_handle, &frame_handle, timeout_ms);
            if (result == K4A_WAIT_RESULT_FAILED)
            {
                throw k4a::error("Failed to pop result from tracker!");
            }
            else if (result == K4A_WAIT_RESULT_TIMEOUT)
            {
                return false;
            }

            *body_frame = frame(frame_handle);
            return true;
        }

        /** Gets the next available body frame. Returns nullptr if timeout
         * Throws error on failure.
         *
         * \sa k4abt_tracker_pop_result
         */
        k4abt::frame pop_result(std::chrono::milliseconds timeout = std::chrono::milliseconds(K4A_WAIT_INFINITE)) const
        {
            k4abt::frame body_frame = nullptr;
            if (pop_result(&body_frame, timeout))
            {
                return body_frame;
            }
            else
            {
                return nullptr;
            }
        }

        /** Set temporal smoothing
         *
         * \sa k4abt_tracker_set_temporal_smoothing
         */
        void set_temporal_smoothing(float smoothing_factor) const noexcept
        {
            k4abt_tracker_set_temporal_smoothing(m_handle, smoothing_factor);
        }

        /** Shut down the k4abt tracker
         *
         * \sa k4abt_tracker_shutdown
         */
        void shutdown() const noexcept
        {
            if (m_handle != nullptr) {
                k4abt_tracker_shutdown(m_handle);
            }
        }

        /** Create a k4abt tracker.
         * Throws error on failure.
         *
         * \sa k4abt_tracker_create
         */
        static tracker create(const k4a_calibration_t& sensor_calibration, const k4abt_tracker_configuration_t& config = K4ABT_TRACKER_CONFIG_DEFAULT)
        {
            k4abt_tracker_t handle = nullptr;
            k4a_result_t result = k4abt_tracker_create(&sensor_calibration, config, &handle);

            if (K4A_RESULT_SUCCEEDED != result)
            {
                throw k4a::error("Failed to create k4abt tracker!");
            }
            return tracker(handle);
        }

    private:
        k4abt_tracker_t m_handle;
    };

    /**
     * @}
     */

} // namespace k4abt

#endif
