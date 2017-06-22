import React from 'react'
import PropTypes from 'prop-types'
import ContentEditorForm from 'forms/ContentEditorForm'

export const ContentEditor = ({ payload, loading, publishing, onSubmit, onSave }) => (
  <ContentEditorForm
    initialValues={payload.data}
    type={payload.type}
    loading={loading}
    onSubmit={onSubmit}
    onSave={onSave} />
)

ContentEditor.propTypes = {
  onSubmit: PropTypes.func.isRequired,
  onSave: PropTypes.func.isRequired,
  payload: PropTypes.object.isRequired,
  loading: PropTypes.bool,
  publishing: PropTypes.bool
}

export default ContentEditor
