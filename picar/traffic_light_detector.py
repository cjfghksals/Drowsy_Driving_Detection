import cv2
import numpy as np

# 전역 변수 설정 (신호등 검출용)
isRed = None
g_cnt = 0
r_cnt = 0

def detectTrafficLight(frame):
    global isRed, g_cnt, r_cnt

    # 프레임의 상단 1/4 ROI 선택
    roi = frame[0:int(frame.shape[0] / 4), :]

    # 그레이스케일로 변환
    gray = cv2.cvtColor(roi, cv2.COLOR_BGR2GRAY)

    # 가우시안 블러 적용
    gray = cv2.GaussianBlur(gray, (9, 9), 2, 2)

    # 허프 변환을 이용해 원 검출
    circles = cv2.HoughCircles(gray, cv2.HOUGH_GRADIENT, 1, gray.shape[0], param1=130, param2=20, minRadius=0, maxRadius=0)

    if circles is not None:
        circles = np.uint16(np.around(circles))
        for i in circles[0, :]:
            center = (i[0], i[1])
            radius = i[2]

            # 원의 중심이 roi의 범위 내에 있는지 확인
            if center[1] >= roi.shape[0] or center[0] >= roi.shape[1]:
                continue

            # 원의 색상 판별
            color = roi[center[1], center[0]]

            # BGR 색상 범위 지정
            blue = color[0]
            green = color[1]
            red = color[2]

            # 빨간색 범위 판별 (BGR 값 기준)
            if red > 110 and green < 110 and blue < 110:
                r_cnt += 1
                g_cnt = 0
                if isRed != 'R' and r_cnt > 20:
                    print("빨간신호입니다.")
                    isRed = 'R'
                cv2.putText(frame, "Red Signal", (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)
            # 초록색 범위 판별 (BGR 값 기준)
            elif green > 90 and red < 90 and blue < 90:
                g_cnt += 1
                r_cnt = 0
                if isRed != 'G' and g_cnt > 20:
                    print("초록신호입니다.")
                    isRed = 'G'
                cv2.putText(frame, "Green Signal", (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

            # 원을 그리기
            cv2.circle(roi, center, radius, (0, 255, 0), 2)

    return frame
