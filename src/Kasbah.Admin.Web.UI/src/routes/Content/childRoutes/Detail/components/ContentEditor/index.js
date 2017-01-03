import React from 'react'
import ContentEditorForm from '../../forms/ContentEditorForm'

export const ContentEditor = ({ payload, loading, onSubmit }) => (
  <ContentEditorForm initialValues={payload.data} type={payload.type} loading={loading} onSubmit={onSubmit} />
)

ContentEditor.propTypes = {
  onSubmit: React.PropTypes.func.isRequired,
  payload: React.PropTypes.object.isRequired,
  loading: React.PropTypes.bool
}

export default ContentEditor
