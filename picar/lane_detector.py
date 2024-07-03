import cv2
import numpy as np

def process_frame(frame, mid_point, threshold):
    # 프레임을 그레이스케일로 변환
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    
    # GaussianBlur를 적용하여 노이즈를 줄이고 가장자리 검출 성능 향상
    blur = cv2.GaussianBlur(gray, (5, 5), 0)
    
    # Canny를 사용한 에지(가장자리) 검출
    edges = cv2.Canny(blur, 50, 150)
    
    # 이미지의 높이와 너비를 정의
    height, width = edges.shape
    # 에지 이미지와 같은 크기의 마스크를 생성
    mask = np.zeros_like(edges)
    
    # 마스크를 위한 다각형을 정의 (프레임의 하단 절반을 덮는 사다리꼴 모양)
    polygon = np.array([[
        (0, height),             # 왼쪽 하단 모서리
        (width, height),         # 오른쪽 하단 모서리
        (width, int(height * 0.6)), # 오른쪽 상단 모서리
        (0, int(height * 0.6))   # 왼쪽 상단 모서리
    ]], np.int32)
    
    # 다각형으로 마스크를 채움
    cv2.fillPoly(mask, polygon, 255)
    
    # 에지 이미지에 마스크를 적용하여 관심 영역만 남김
    masked_edges = cv2.bitwise_and(edges, mask)
    
    # Hough 변환을 사용하여 선 검출
    lines = cv2.HoughLinesP(masked_edges, rho=1, theta=np.pi/180, threshold=50, minLineLength=100, maxLineGap=50)
    
    left_lines = []  # 왼쪽 차선 선들을 저장할 리스트
    right_lines = [] # 오른쪽 차선 선들을 저장할 리스트
    
    if lines is not None:
        for line in lines:
            for x1, y1, x2, y2 in line:
                # 선의 기울기를 계산
                slope = (y2 - y1) / (x2 - x1) if (x2 - x1) != 0 else 0
                # 선을 기울기에 따라 분류
                if slope < -0.5:  # 왼쪽 차선 (기울기 음수)
                    left_lines.append(line)
                elif slope > 0.5:  # 오른쪽 차선 (기울기 양수)
                    right_lines.append(line)
    
    def average_lines(lines):
        x_coords = []
        y_coords = []
        for line in lines:
            for x1, y1, x2, y2 in line:
                # 선의 x, y 좌표를 수집
                x_coords += [x1, x2]
                y_coords += [y1, y2]
        if len(x_coords) == 0:
            return None
        # 수집된 좌표로 선을 맞춤
        poly = np.polyfit(x_coords, y_coords, 1)
        slope, intercept = poly
        y1 = height
        y2 = int(height * 0.6)
        x1 = int((y1 - intercept) / slope)
        x2 = int((y2 - intercept) / slope)
        return [[x1, y1, x2, y2]]
    
    left_line = average_lines(left_lines)   # 왼쪽 차선 선들을 평균화
    right_line = average_lines(right_lines) # 오른쪽 차선 선들을 평균화
    
    # 선을 그리기 위한 이미지 생성
    line_image = np.zeros_like(frame)
    
    if left_line is not None:
        for x1, y1, x2, y2 in left_line:
            # 왼쪽 차선 선을 녹색으로 그림
            cv2.line(line_image, (x1, y1), (x2, y2), (0, 255, 0), 5)
    
    if right_line is not None:
        for x1, y1, x2, y2 in right_line:
            # 오른쪽 차선 선을 녹색으로 그림
            cv2.line(line_image, (x1, y1), (x2, y2), (0, 255, 0), 5)
    
    # 원본 프레임과 선 이미지를 결합
    combo = cv2.addWeighted(frame, 0.8, line_image, 1, 1)
    
    # 관심 영역 다각형을 빨간색으로 그림
    cv2.polylines(combo, polygon, isClosed=True, color=(0, 0, 255), thickness=2)
    
    # 중간 지점을 빨간색으로 그림
    cv2.line(combo, (mid_point, height), (mid_point, int(height * 0.6)), (0, 0, 255), 2)
    
    # 차선 이탈 감지를 위한 기준선을 빨간색으로 그림
    left_threshold = mid_point - threshold
    right_threshold = mid_point + threshold
    cv2.line(combo, (left_threshold, height), (left_threshold, int(height * 0.6)), (0, 0, 255), 2)
    cv2.line(combo, (right_threshold, height), (right_threshold, int(height * 0.6)), (0, 0, 255), 2)
    
    # 차선 이탈 여부를 확인하고 차선 중앙을 그림
    if left_line is not None and right_line is not None:
        left_mid = (left_line[0][0] + left_line[0][2]) // 2
        right_mid = (right_line[0][0] + right_line[0][2]) // 2
        lane_center = (left_mid + right_mid) // 2
        
        # 검출된 차선 중앙을 녹색으로 그림
        cv2.line(combo, (lane_center, height), (lane_center, int(height * 0.6)), (0, 255, 0), 2)
        
        # 차선 중앙이 기준선 밖에 있으면 경고 메시지 표시
        if abs(lane_center - mid_point) > threshold:
            cv2.putText(combo, "Lane Departure!", (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2, cv2.LINE_AA)
        # 차선 중앙값을 표시
        cv2.putText(combo, f"Lane Center: {lane_center}", (50, 80), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2, cv2.LINE_AA)
    
    return combo
