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
            <span class="text-xl font-bold text-gray-800">库房管理</span>
          </div>
          <button @click="showCreateModal = true" class="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors">
            <svg class="w-5 h-5 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
            </svg>
            新建库房
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
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">编码</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">名称</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">位置</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">备注</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">状态</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">操作</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="item in warehouses" :key="item.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">{{ item.code }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.name }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ item.location || '-' }}</td>
                <td class="px-6 py-4">{{ item.remark || '-' }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="item.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'" class="px-2 py-1 text-xs font-medium rounded-full">
                    {{ item.isActive ? '启用' : '禁用' }}
                  </span>
                </td>
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
          <span class="text-sm text-gray-600">显示 {{ warehouses.length }} 条记录</span>
        </div>
      </div>
    </div>

    <div v-if="showCreateModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl w-full max-w-md mx-4">
        <div class="px-6 py-4 border-b">
          <h3 class="text-lg font-semibold text-gray-800">{{ editingItem ? '编辑库房' : '新建库房' }}</h3>
        </div>
        <div class="px-6 py-4 space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">编码</label>
            <input v-model="formData.code" type="text" placeholder="请输入编码" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">名称</label>
            <input v-model="formData.name" type="text" placeholder="请输入名称" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">位置</label>
            <input v-model="formData.location" type="text" placeholder="请输入位置" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">备注</label>
            <textarea v-model="formData.remark" placeholder="请输入备注" rows="3" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"></textarea>
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
import { warehouseApi } from '@/api/warehouse';
import type { WarehouseDto } from '@/api/warehouse'

const router = useRouter()

const warehouses = ref<WarehouseDto[]>([])
const showCreateModal = ref(false)
const editingItem = ref<WarehouseDto | null>(null)

const formData = reactive({
  code: '',
  name: '',
  location: '',
  remark: ''
})

const loadWarehouses = async () => {
  try {
    const result = await warehouseApi.getAll()
    warehouses.value = result || []
  } catch (e) {
    console.error('Failed to load warehouses:', e)
    warehouses.value = []
  }
}

const handleEdit = (item: WarehouseDto) => {
  editingItem.value = item
  formData.code = item.code
  formData.name = item.name
  formData.location = item.location || ''
  formData.remark = item.remark || ''
  showCreateModal.value = true
}

const handleDelete = async (id: number) => {
  if (!confirm('确定要删除这个库房吗？')) return
  try {
    await warehouseApi.delete(id)
    loadWarehouses()
  } catch (e) {
    console.error('Failed to delete warehouse:', e)
  }
}

const handleSave = async () => {
  try {
    if (editingItem.value) {
      await warehouseApi.update(editingItem.value.id, formData)
    } else {
      await warehouseApi.create(formData)
    }
    showCreateModal.value = false
    editingItem.value = null
    formData.code = ''
    formData.name = ''
    formData.location = ''
    formData.remark = ''
    loadWarehouses()
  } catch (e) {
    console.error('Failed to save warehouse:', e)
  }
}

onMounted(() => {
  loadWarehouses()
})
</script>
