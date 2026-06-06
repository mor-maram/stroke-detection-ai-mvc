import os
from pathlib import Path
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense, Conv2D, MaxPooling2D, Flatten, Dropout
from tensorflow.keras.preprocessing.image import ImageDataGenerator

# تحديد المسارات
train_dir = r'C:\Users\meme7\Downloads\archive (1)\Brain_Stroke_CT-SCAN_image\Train'
validation_dir = r'C:\Users\meme7\Downloads\archive (1)\Brain_Stroke_CT-SCAN_image'

# إعداد معالج الصور
train_datagen = ImageDataGenerator(rescale=1./255)
validation_datagen = ImageDataGenerator(rescale=1./255)

# تحميل الصور مع التحقق
train_generator = train_datagen.flow_from_directory(
    train_dir,
    target_size=(224, 224),  # تعديل الحجم حسب حاجتك
    batch_size=32,
    class_mode='categorical'
)

validation_generator = validation_datagen.flow_from_directory(
    validation_dir,
    target_size=(224, 224),
    batch_size=32,
    class_mode='categorical'
)

# عدد الفئات بناءً على عدد المجلدات
num_classes = len(train_generator.class_indices)
print(f"عدد الفئات: {num_classes}")

# بناء النموذج
model = Sequential([
    Conv2D(32, (3, 3), activation='relu', input_shape=(224, 224, 3)),
    MaxPooling2D(pool_size=(2, 2)),
    
    Conv2D(64, (3, 3), activation='relu'),
    MaxPooling2D(pool_size=(2, 2)),
    
    Flatten(),
    Dense(128, activation='relu'),
    Dropout(0.5),
    Dense(num_classes, activation='softmax')
])
model.compile(
    optimizer='adam',
    loss='categorical_crossentropy',
    metrics=['accuracy']
)

#sgd
# model.compile(
#     optimizer=tf.keras.optimizers.SGD(learning_rate=0.01, momentum=0.9),
#     loss='categorical_crossentropy',
#     metrics=['accuracy']
# )

#RMSprop
# model.compile(
#     optimizer=tf.keras.optimizers.RMSprop(learning_rate=0.001),
#     loss='categorical_crossentropy',
#     metrics=['accuracy']
# )

# model.compile(
#     optimizer=tf.keras.optimizers.Adagrad(learning_rate=0.01),
#     loss='categorical_crossentropy',
#     metrics=['accuracy']
# )

#تجميع النموذج
model.compile(
    optimizer='adam',
    loss='categorical_crossentropy',
    metrics=['accuracy']
)

# تلخيص النموذج
model.summary()

# تدريب النموذج
model.fit(
    train_generator,
    epochs=100,
    validation_data=validation_generator
)

# 5. حفظ النموذج
model.save('D:/Workspaces/Python/StrokeProjrct/brain_stroke_env/BrainStrokePrediction/model/BrainStrokePrediction.h5')
# حفظ النموذج المدرب
#model.save('D:/Workspaces/Python/StrokeProjrct/brain_stroke_env/BrainStrokePrediction/model/BrainStrokePrediction_model')

print("✅ تم حفظ النموذج بنجاح.")
