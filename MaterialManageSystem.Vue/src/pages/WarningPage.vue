<template>
  <div class="min-h-screen bg-gray-50">
    <nav class="bg-white shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <div class="flex items-center">
            <button @click="router.push('/')" class="mr-4">
              <svg class="w-6 h-6 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"/>
              </svg>
            </button>
            <span class="text-xl font-bold text-gray-800">预警管理</span>
          </div>
        </div>
      </div>
    </nav>

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="bg-white rounded-xl shadow-sm overflow-hidden">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">预警类型</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">物料编码</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">料卷编号</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">预警描述</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">预警时间</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">状态</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="item in warnings" :key="item.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="getWarningTypeClass(item.warningType)" class="px-2 py-1 text-xs font-medium rounded-full">
                    {{ getWarningTypeName(item.warningType) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.partNoName }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.reelNo }}</td>
                <td class="px-6 py-4">{{ item.message }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.createdAt }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="item.isHandled ? 'bg-gray-100 text-gray-800' : 'bg-red-100 text-red-800'" class="px-2 py-1 text-xs font-medium rounded-full">
                    {{ item.isHandled ? '已处理' : '未处理' }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="px-6 py-4 bg-gray-50 flex justify-between items-center">
          <span class="text-sm text-gray-600">显示 {{ warnings.length }} 条记录</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { dashboardApi } from '@/api/dashboard'

interface WarningItem {
  id: number
  reelId: number
  reelNo: string
  partNoName: string
  warningType: number
  warningLevel: number
  message: string
  createdAt: string
  isHandled?: boolean
}

interface WarningSummaryData {
  totalWarnings: number
  criticalWarnings: number
  normalWarnings: number
  warnings: WarningItem[]
}

const router = useRouter()
const warnings = ref<WarningItem[]>([])

const getWarningTypeName = (type: number) => {
  switch (type) {
    case 0:
      return '库存不足'
    case 1:
      return '过期预警'
    case 2:
      return '长时间未使用'
    case 3:
      return '即将到期'
    default:
      return '未知'
  }
}

const getWarningTypeClass = (type: number) => {
  switch (type) {
    case 0:
      return 'bg-red-100 text-red-800'
    case 1:
      return 'bg-orange-100 text-orange-800'
    case 2:
      return 'bg-yellow-100 text-yellow-800'
    case 3:
      return 'bg-blue-100 text-blue-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const loadWarnings = async () => {
  try {
    const result = await dashboardApi.getWarningSummary()
    if (Array.isArray(result)) {
      warnings.value = result
    } else if (result && typeof result === 'object') {
      warnings.value = result.warnings || []
    }
  } catch (e) {
    console.error('Failed to load warnings:', e)
  }
}

onMounted(() => {
  loadWarnings()
})
</script>
