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
            <span class="text-xl font-bold text-gray-800">料卷管理</span>
          </div>
          <button @click="showCreateModal = true" class="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors">
            <svg class="w-5 h-5 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
            </svg>
            新建料卷
          </button>
        </div>
      </div>
    </nav>

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="bg-white rounded-xl shadow-sm overflow-hidden">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">料卷编号</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">物料编码</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">物料名称</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">初始数量</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">当前数量</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">状态</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">操作</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="item in reels" :key="item.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">{{ item.reelNo }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.partNoCode }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.partNoName }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.initialQuantity }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.currentQuantity }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="getStatusClass(item.status)" class="px-2 py-1 text-xs font-medium rounded-full">
                    {{ item.statusName }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <button v-if="item.status === 0" @click="handleCheckout(item)" class="text-orange-600 hover:text-orange-800 mr-2">
                    出库
                  </button>
                  <button v-if="item.status === 1" @click="handleOnline(item)" class="text-green-600 hover:text-green-800 mr-2">
                    上线
                  </button>
                  <button v-if="item.status === 2" @click="handleReturn(item)" class="text-blue-600 hover:text-blue-800 mr-2">
                    退回
                  </button>
                  <button v-if="item.status !== 3" @click="handleScrap(item)" class="text-red-600 hover:text-red-800">
                    报废
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="px-6 py-4 bg-gray-50 flex justify-between items-center">
          <span class="text-sm text-gray-600">显示 {{ reels.length }} 条记录</span>
        </div>
      </div>
    </div>

    <div v-if="showCreateModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl w-full max-w-lg mx-4">
        <div class="px-6 py-4 border-b">
          <h3 class="text-lg font-semibold text-gray-800">新建料卷</h3>
        </div>
        <div class="px-6 py-4 space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">料卷编号</label>
            <input v-model="formData.reelNo" type="text" placeholder="请输入料卷编号" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">物料编码</label>
            <input v-model="formData.partNoId" type="number" placeholder="请输入物料ID" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">初始数量</label>
            <input v-model.number="formData.initialQuantity" type="number" placeholder="请输入初始数量" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">生产日期</label>
            <input v-model="formData.manufactureDate" type="date" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">有效期至</label>
            <input v-model="formData.expiryDate" type="date" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
        </div>
        <div class="px-6 py-4 border-t flex justify-end space-x-3">
          <button @click="showCreateModal = false" class="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg">取消</button>
          <button @click="handleCreate" class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700">保存</button>
        </div>
      </div>
    </div>

    <div v-if="showActionModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl w-full max-w-md mx-4">
        <div class="px-6 py-4 border-b">
          <h3 class="text-lg font-semibold text-gray-800">{{ actionTitle }}</h3>
        </div>
        <div class="px-6 py-4 space-y-4">
          <div v-if="currentAction === 'checkout' || currentAction === 'online'">
            <label class="block text-sm font-medium text-gray-700 mb-1">数量</label>
            <input v-model.number="actionData.quantity" type="number" placeholder="请输入数量" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">员工ID</label>
            <input v-model.number="actionData.employeeId" type="number" placeholder="请输入员工ID" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">备注</label>
            <textarea v-model="actionData.remark" placeholder="请输入备注" rows="3" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"></textarea>
          </div>
        </div>
        <div class="px-6 py-4 border-t flex justify-end space-x-3">
          <button @click="showActionModal = false" class="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg">取消</button>
          <button @click="handleAction" class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700">确认</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { reelApi } from '@/api/reel';
import type { ReelIdDto, CreateReelRequest } from '@/api/reel'

const router = useRouter()

const reels = ref<ReelIdDto[]>([])
const showCreateModal = ref(false)
const showActionModal = ref(false)
const currentAction = ref('')
const currentReel = ref<ReelIdDto | null>(null)
const actionTitle = ref('')

const formData = reactive<CreateReelRequest>({
  reelNo: '',
  partNoId: 0,
  initialQuantity: 0,
  manufactureDate: undefined,
  expiryDate: undefined
})

const actionData = reactive({
  employeeId: 0,
  quantity: undefined as number | undefined,
  remark: ''
})

const getStatusClass = (status: number) => {
  switch (status) {
    case 0:
      return 'bg-green-100 text-green-800'
    case 1:
      return 'bg-yellow-100 text-yellow-800'
    case 2:
      return 'bg-blue-100 text-blue-800'
    case 3:
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const loadReels = async () => {
  try {
    const result = await reelApi.getAll()
    reels.value = result || []
  } catch (e) {
    console.error('Failed to load reels:', e)
    reels.value = []
  }
}

const handleCreate = async () => {
  try {
    await reelApi.create(formData)
    showCreateModal.value = false
    formData.reelNo = ''
    formData.partNoId = 0
    formData.initialQuantity = 0
    formData.manufactureDate = undefined
    formData.expiryDate = undefined
    loadReels()
  } catch (e) {
    console.error('Failed to create reel:', e)
  }
}

const handleCheckout = (item: ReelIdDto) => {
  currentReel.value = item
  currentAction.value = 'checkout'
  actionTitle.value = '出库操作'
  actionData.employeeId = 0
  actionData.quantity = item.currentQuantity
  actionData.remark = ''
  showActionModal.value = true
}

const handleOnline = (item: ReelIdDto) => {
  currentReel.value = item
  currentAction.value = 'online'
  actionTitle.value = '上线操作'
  actionData.employeeId = 0
  actionData.quantity = item.currentQuantity
  actionData.remark = ''
  showActionModal.value = true
}

const handleReturn = (item: ReelIdDto) => {
  currentReel.value = item
  currentAction.value = 'return'
  actionTitle.value = '退回操作'
  actionData.employeeId = 0
  actionData.quantity = undefined
  actionData.remark = ''
  showActionModal.value = true
}

const handleScrap = (item: ReelIdDto) => {
  currentReel.value = item
  currentAction.value = 'scrap'
  actionTitle.value = '报废操作'
  actionData.employeeId = 0
  actionData.quantity = undefined
  actionData.remark = ''
  showActionModal.value = true
}

const handleAction = async () => {
  if (!currentReel.value) return
  try {
    switch (currentAction.value) {
      case 'checkout':
        await reelApi.checkout(currentReel.value.id, {
          employeeId: actionData.employeeId,
          quantity: actionData.quantity,
          remark: actionData.remark
        })
        break
      case 'online':
        await reelApi.online(currentReel.value.id, {
          employeeId: actionData.employeeId,
          quantity: actionData.quantity || 0,
          remark: actionData.remark
        })
        break
      case 'return':
        await reelApi.return(currentReel.value.id, {
          employeeId: actionData.employeeId,
          quantity: actionData.quantity || 0,
          remark: actionData.remark
        })
        break
      case 'scrap':
        await reelApi.scrap(currentReel.value.id, {
          employeeId: actionData.employeeId,
          remark: actionData.remark
        })
        break
    }
    showActionModal.value = false
    loadReels()
  } catch (e) {
    console.error('Failed to perform action:', e)
  }
}

onMounted(() => {
  loadReels()
})
</script>
