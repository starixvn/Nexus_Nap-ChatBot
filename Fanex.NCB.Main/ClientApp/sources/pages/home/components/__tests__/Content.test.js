import React from 'react';
import { Content } from './../content';
import { configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import ShallowRenderer from 'react-test-renderer/shallow';

configure({ adapter: new Adapter() })

describe('Content', () => {
    describe('render', () => {
        it('sidebar is expanded', () => {
            // Arrange
            const props = {
                isSidebarExpanded: true,
            }

            // Act
            const renderOutput = (new ShallowRenderer()).render(<Content {...props}/>)

            // Assert
            expect(renderOutput).toMatchSnapshot()
        })

        it('sidebar is not expanded', () => {
            // Arrange
            const props = {
                isSidebarExpanded: false,
            }

            // Act
            const renderOutput = (new ShallowRenderer()).render(<Content {...props}/>)

            // Assert
            expect(renderOutput).toMatchSnapshot()
        })
    })
})
