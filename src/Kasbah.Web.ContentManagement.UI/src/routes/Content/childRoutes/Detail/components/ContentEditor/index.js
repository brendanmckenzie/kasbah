import React from 'react'
import ContentEditorForm from '../../forms/ContentEditorForm'

export const ContentEditor = ({ payload, loading, publishing, onSubmit, onSave }) => (
  <ContentEditorForm
    initialValues={payload.data}
    type={payload.type}
    loading={loading}
    onSubmit={onSubmit}
    onSave={onSave} />
)

ContentEditor.propTypes = {
  onSubmit: React.PropTypes.func.isRequired,
  onSave: React.PropTypes.func.isRequired,
  payload: React.PropTypes.object.isRequired,
  loading: React.PropTypes.bool,
  publishing: React.PropTypes.bool
}

export default ContentEditor
