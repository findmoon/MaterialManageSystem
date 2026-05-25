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
            <span class="text-xl font-bold text-gray-800">物料管理</span>
          </div>
          <button @click="showCreateModal = true" class="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors">
            <svg class="w-5 h-5 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
            </svg>
            新建物料
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
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">物料编码</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">名称</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">规格</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">单位</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">总库存</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">预警值</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">操作</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="item in partNos" :key="item.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">{{ item.partNoCode }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.name }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.specification || '-' }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.unit }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.totalQuantity }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.warningQuantity || '-' }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <button @click="handleEdit(item)" class="text-blue-600 hover:text-blue-800 mr-3">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                    </svg>
                  </button>
                  <button @click="handleDelete(item.id)" class="text-red-600 hover:text-red-800">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                    </svg>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="px-6 py-4 bg-gray-50 flex justify-between items-center">
          <span class="text-sm text-gray-600">显示 {{ partNos.length }} 条记录</span>
        </div>
      </div>
    </div>

    <div v-if="showCreateModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl w-full max-w-lg mx-4">
        <div class="px-6 py-4 border-b">
          <h3 class="text-lg font-semibold text-gray-800">{{ editingItem ? '编辑物料' : '新建物料' }}</h3>
        </div>
        <div class="px-6 py-4 space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">物料编码</label>
            <input v-model="formData.partNoCode" type="text" placeholder="请输入物料编码" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">名称</label>
            <input v-model="formData.name" type="text" placeholder="请输入名称" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">规格</label>
            <input v-model="formData.specification" type="text" placeholder="请输入规格" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">单位</label>
            <input v-model="formData.unit" type="text" placeholder="请输入单位" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">预警数量</label>
            <input v-model.number="formData.warningQuantity" type="number" placeholder="请输入预警数量" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">预警天数</label>
            <input v-model.number="formData.warningDays" type="number" placeholder="请输入预警天数" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
        </div>
        <div class="px-6 py-4 border-t flex justify-end space-x-3">
          <button @click="showCreateModal = false" class="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg">取消</button>
          <button @click="handleSave" class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700">保存</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { partNoApi } from '@/api/partno';
import type { PartNoDto } from '@/api/partno'

const router = useRouter()

const partNos = ref<PartNoDto[]>([])
const showCreateModal = ref(false)
const editingItem = ref<PartNoDto | null>(null)

const formData = reactive({
  partNoCode: '',
  name: '',
  specification: '',
  unit: '',
  warningQuantity: undefined as number | undefined,
  warningDays: undefined as number | undefined
})

const loadPartNos = async () => {
  try {
    const result = await partNoApi.getAll()
    partNos.value = result || []
  } catch (e) {
    console.error('Failed to load partNos:', e)
    partNos.value = []
  }
}

const handleEdit = (item: PartNoDto) => {
  editingItem.value = item
  formData.partNoCode = item.partNoCode
  formData.name = item.name
  formData.specification = item.specification || ''
  formData.unit = item.unit
  formData.warningQuantity = item.warningQuantity
  formData.warningDays = item.warningDays
  showCreateModal.value = true
}

const handleDelete = async (id: number) => {
  if (!confirm('确定要删除这个物料吗？')) return
  try {
    await partNoApi.delete(id)
    loadPartNos()
  } catch (e) {
    console.error('Failed to delete partNo:', e)
  }
}

const handleSave = async () => {
  try {
    if (editingItem.value) {
      await partNoApi.update(editingItem.value.id, formData)
    } else {
      await partNoApi.create(formData)
    }
    showCreateModal.value = false
    editingItem.value = null
    formData.partNoCode = ''
    formData.name = ''
    formData.specification = ''
    formData.unit = ''
    formData.warningQuantity = undefined
    formData.warningDays = undefined
    loadPartNos()
  } catch (e) {
    console.error('Failed to save partNo:', e)
  }
}

onMounted(() => {
  loadPartNos()
})
</script>
