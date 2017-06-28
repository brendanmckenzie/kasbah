import React from 'react'
import PropTypes from 'prop-types'
import { Field, reduxForm } from 'redux-form'
import _ from 'lodash'
import { Tabs, Tab } from 'components/Tabs'
import kasbah from 'kasbah'

class ContentEditorForm extends React.Component {
  static propTypes = {
    handleSubmit: PropTypes.func.isRequired,
    type: PropTypes.object.isRequired,
    loading: PropTypes.bool
  }

  render() {
    const { handleSubmit, type, loading } = this.props

    return (
      <form className='content-editor__form' disabled={loading} onSubmit={handleSubmit}>
        <Tabs>
          {_(type.fields).map(ent => ent.category).uniq().value().map((ent, index) => (
            <Tab key={index} title={ent}>
              <div>
                {type.fields.filter(fld => fld.category === ent).map((fld, fldIndex) => (
                  <div key={fldIndex} className='field'>
                    <label className='label' htmlFor={fld.alias}>{fld.displayName}</label>
                    <div className='control'>
                      <Field
                        name={fld.alias}
                        id={fld.alias}
                        component={kasbah.getEditor(fld.editor)}
                        options={fld.options}
                        type={fld.type}
                        className='input' />
                      {fld.helpText && <span className='help'>{fld.helpText}</span>}
                    </div>
                  </div>
                ))}
              </div>
            </Tab>
          ))}
        </Tabs>
        <hr />
        <div className='level'>
          <div className='level-left' />
          <div className='level-right'>
            <button className={'level-item button is-primary' + (loading ? ' is-loading' : '')}
              type='submit'>Save</button>
          </div>
        </div>
      </form>
    )
  }
}

export default reduxForm({ form: 'ContentEditorForm' })(ContentEditorForm)
